using System;
using Core;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Crawler : MonoBehaviour, ILightResponder
{
    private enum State
    {
        Patrolling,
        Dropping,
        Chasing,
        Attacking,
        Returning
    }

    [SerializeField] private float sanityDamage = 10f;
    [SerializeField] private float movementSpeed = 3f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private int floorLayerID;
    [SerializeField] private float playerCheckRadius = 10f;
    [SerializeField] private float attackRange = 10f;
    [SerializeField] private Transform[] patrolPoints;

    private Vector2 goal;
    private Vector2 start;
    private State currentState = State.Patrolling;
    private bool canAttack = true;
    private SpriteRenderer renderer;
    private Rigidbody2D rigidbody;
    private CharacterController player;
    private bool isdropping = false;
    private bool isChasing = false;
    private bool isGrounded = false;
    private bool patrolPointReached = false;
    private bool isInLight = false;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    private void Start()
    {
        player = FindObjectOfType<CharacterController>();
        if (patrolPoints.Length > 0)
        {
            transform.position = patrolPoints[0].position;
            if (patrolPoints.Length > 1)
            {
                goal = patrolPoints[1].position;
            }
        }
    }

    private void Update()
    {
        if (currentState == State.Patrolling)
        {
            isGrounded = false;
            if (Physics2D.CircleCast(transform.position, playerCheckRadius,Vector2.down, 100f, playerMask))
            {
                currentState = State.Dropping;
                return;
            }
            
            if (CloseEnough(transform.position, goal))
            {
                patrolPointReached = true;
            }
            
            if (patrolPointReached)
            {
                SetNewPatrolGoal();
            }
        }
        else if (currentState == State.Dropping)
        {
            if (!isdropping)
            {
                isdropping = true;
                renderer.flipY = !renderer.flipY;
                rigidbody.bodyType = RigidbodyType2D.Dynamic;
            }
            goal = transform.position + Vector3.down;
        }
        else if (currentState == State.Chasing)
        {
            isdropping = false;
            if (!isChasing)
            {
                isChasing = true;
                rigidbody.bodyType = RigidbodyType2D.Kinematic;
            }

            goal = player.transform.position;
            
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (PlayerInRange(distance))
            {
                currentState = State.Attacking;
                isChasing = false;
            }
        }
        else if (currentState == State.Attacking)
        {
            goal = transform.position;
            if (canAttack)
            {
                canAttack = false;
                Attack();
                currentState = State.Returning;
                canAttack = true;
            }
            
        }
        else if (currentState == State.Returning)
        {
            goal = patrolPoints[0].position;

            if (CloseEnough(transform.position, goal)) 
            {
                renderer.flipY = !renderer.flipY;
                currentState = State.Patrolling;
            }
        }

        UpdateFacingDirection();
    }
    
    private void FixedUpdate()
    {
        transform.position = Vector2.MoveTowards(transform.position, goal, Time.deltaTime * movementSpeed);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject == player.gameObject)
        {
            rigidbody.velocity = Vector2.zero;
            rigidbody.bodyType = RigidbodyType2D.Kinematic;
            isdropping = false;
            isChasing = false;
            canAttack = true;
            currentState = State.Attacking;
        }
        
        if (other.gameObject.layer == floorLayerID)
        {
            if (!isGrounded)
            {
                isGrounded = true;
                currentState = State.Chasing;
            }
        }
    }

    private void UpdateFacingDirection()
    {
        renderer.flipX = transform.position.x > goal.x;
    }

    private void SetNewPatrolGoal()
    {
        for (int i = 0; i < patrolPoints.Length; i++)
        {
            if (patrolPoints[i].position.Equals(goal))
            {
                if (patrolPoints.Length > i+1)
                {
                    goal = patrolPoints[i + 1].position;
                    patrolPointReached = false;
                    return;
                }
                else
                {
                    goal = patrolPoints[0].position;
                    patrolPointReached = false;
                    return;
                }
            }
        }

        goal = transform.position;
    }

    private bool CloseEnough(Vector2 v1, Vector2 v2)
    {
        return MathF.Abs(v1.y - v2.y) < 0.2f && Mathf.Abs(v1.x - v2.x ) < 0.2f;
    }
    
    private bool PlayerInRange(float distance)
    {
        return distance <= attackRange;
    }
    
    private void  Attack()
    {
        if (player != null)
        {
            Debug.Log("attacking");
            player.GetCharacterSanity.DecreaseSanity(sanityDamage, false);
        }
    }
    public void OnLightEntered(float intensity) //Place in dark areas as this will stay out of any light
    {
        if (isInLight) return;
        rigidbody.velocity = Vector2.zero;
        rigidbody.bodyType = RigidbodyType2D.Kinematic;
        isdropping = false;
        isChasing = false;
        canAttack = true;
        currentState = State.Returning;
    }

    public void OnLightExited(float intensity)
    {
        isInLight = false;
    }

    public void OnLightStay(float intensity)
    {
        //nothing
    }
}