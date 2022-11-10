using System;
using System.Collections;
using Character;
using Core;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace AI
{
    public class SM01 : MonoBehaviour, ILightResponder
    {
        [SerializeField] private float moveVelocity = 10;
        [SerializeField] private float stopDistance = 5;
        [SerializeField] private float maxWaypointStopTime = 3;
        [SerializeField] private float attackCooldown = 3;
        [SerializeField] private Vector2[] patrolPoints;
        [SerializeField] public Animator animator;
        [SerializeField] private Transform puppet;
        [SerializeField] private LayerMask lightMask;
        private Vector3 startPosition;

        [SerializeField] private Vector2 boxCastSize;

        private Collider2D bodyCollider;

        [Header("Movement")]
        private Vector2 direction = new (-1, 1);
        private bool shouldMove = true;

        private RaycastHit2D boxCastHit;

        private GameObject targetPlayer;

        private bool isPlayerInRange = false;
        private bool isAttacking;
        private bool isAtWaypointStop;
        private int currentPoint;
        private readonly Vector3 flippedScale = new(-1, 1, 1);

        // Start is called before the first frame update
        private void Awake()
        {
            bodyCollider = GetComponent<Collider2D>();
            var lc = FindObjectOfType<LevelController>();
            lc.PlayerSpawned.AddListener(onPlayerLoaded);
            
        }

        private void onPlayerLoaded(CharacterController controller)
        {
            targetPlayer = GameObject.FindWithTag("Player");
        }

        private void Start()
        {
            startPosition = transform.position;
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void FixedUpdate()
        {
            Move();
            UpdateDirection();
            BoxCastPlayerDetection();
        }
        
        private void Move()
        {
            var currentPosition = transform.position;
            switch (shouldMove)
            {
                case true when !isPlayerInRange:
                {
                    // How far away are we from the target?
                    var distance = (patrolPoints[currentPoint] - (Vector2)currentPosition).magnitude;

                    // If we are closer to our target than our minimum distance...
                    if (distance <= stopDistance)
                    {
                        // Update to the next target
                        currentPoint += 1;
                        
                        // If we've gone past the end of our list...
                        // (if our current point index is equal or bigger than
                        // the length of our list)
                        if (currentPoint >= patrolPoints.Length)
                        {
                            // ...loop back to the start by setting 
                            // the current point index to 0
                            currentPoint = 0;
                        }
                        shouldMove = false;
                        animator.SetBool("IsMoving", false);
                        break;
                    }
                    
                    if (shouldMove && !isPlayerInRange)
                    {
                        // Now, move in the direction of our target
                        // Get the direction    
                        // Subtract the current position from the target position to get a distance vector
                        // Normalise changes it to be length 1, so we can then multiply it by our speed / force
                        direction = (patrolPoints[currentPoint] - (Vector2)currentPosition).normalized;
                        animator.SetBool("IsMoving", true);
                        Vector2 targetPosition = new Vector2(patrolPoints[currentPoint].x, currentPosition.y);
                        currentPosition = Vector2.MoveTowards(currentPosition, targetPosition, moveVelocity * Time.deltaTime);
                        transform.position = currentPosition;
                    }
                    break;
                }
                case true when isPlayerInRange:
                {
                    var position = transform.position;
                    Vector2 targetPosition = new Vector2(targetPlayer.transform.position.x, position.y);
                    var distance = (targetPosition - (Vector2)position).magnitude;
                    direction = (targetPosition - (Vector2)position).normalized;
                    Debug.Log(distance.ToString());
                    position = Vector2.MoveTowards(position, targetPosition, moveVelocity * Time.deltaTime);
                    transform.position = position;
                    if (distance >= boxCastSize.x - 0.09f * boxCastSize.x)
                    {
                        isPlayerInRange = false;
                    }
                    break;
                }
            }

            if (!shouldMove && !isAtWaypointStop && !isAttacking)
            {
                StartCoroutine(WayPointStopTime());
            }
        }

        private void UpdateDirection()
        {
            if (direction.x >= 0)
            {
                if (puppet)
                {
                    puppet.localScale = flippedScale;
                    return;
                }
            }
            
            if (direction.x <= 0)
            {
                if (puppet)
                {
                    puppet.localScale = Vector2.one;
                }
            } 
        }
        private void BoxCastPlayerDetection()
        {
            boxCastHit = Physics2D.BoxCast(transform.position + new Vector3(boxCastSize.x * 0.5f * direction.x, -boxCastSize.y, 0), boxCastSize, 0, direction);
            
            if (boxCastHit.collider.gameObject.CompareTag("Player"))
            { 
                Debug.Log("Player in range");
                animator.SetBool("IsMoving", true);
                isPlayerInRange = true;
                shouldMove = true;
            }
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if ((lightMask & 1 << other.gameObject.layer) == 1 << other.gameObject.layer)
            {
                Debug.Log("Enemy is in the light");
                Destroy(gameObject);
            }
            
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            var cc = col.gameObject.GetComponent<CharacterSanity>();
            if (cc)
            {
                cc.DecreaseSanity(100f, false);
            }
        }

        private IEnumerator AttackCooldownReset()
        {
            yield return new WaitForSeconds(attackCooldown);
        }
        
        private IEnumerator WayPointStopTime()
        {
            isAtWaypointStop = true;
            yield return new WaitForSeconds(Random.Range(1, maxWaypointStopTime));
            shouldMove = true;
            isAtWaypointStop = false;
        }

        public void OnLightEntered(float intensity)
        {
            Debug.Log("Enemy is in the light");
            Destroy(gameObject);
        }

        public void OnLightExited(float intensity)
        {
        }

        public void OnLightStay(float intensity)
        {
        }

        private void OnDrawGizmos()
        {
            if (patrolPoints.Length == 0) return;
            foreach (Vector3 pos in patrolPoints)
            {
                var gizPos = pos;
                gizPos.y = transform.position.y;
                Gizmos.DrawWireCube(gizPos, Vector3.one);
            }
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + new Vector3(boxCastSize.x * 0.5f * direction.x, boxCastSize.y * -0.5f, 0), boxCastSize);
        }
    }
}
