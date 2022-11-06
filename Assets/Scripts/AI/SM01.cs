using System;
using System.Collections;
using Character;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Random = UnityEngine.Random;

namespace AI
{
    public class SM01 : MonoBehaviour
    {
        [SerializeField] private float moveVelocity = 10;
        [SerializeField] private float stopDistance = 5;
        [SerializeField] private float maxWaypointStopTime = 3;
        [SerializeField] private float attackCooldown = 3;
        [SerializeField] private Vector2[] patrolPoints;
        [SerializeField] public Animator animator;
        [SerializeField] private Transform puppet;
        [SerializeField] private LayerMask lightMask;

        private Collider2D bodyCollider;
        
        [Header("Movement")]
        private Vector2 direction;
        private bool shouldMove;

        private bool isAttacking;
        private bool isAtWaypointStop;
        private int currentPoint;
        private readonly Vector3 flippedScale = new(-1, 1, 1);

        // Start is called before the first frame update
        private void Awake()
        {
            bodyCollider = GetComponent<Collider2D>();
        }

        // Update is called once per frame
        private void Update()
        {
        }

        private void FixedUpdate()
        {
            Move();
            UpdateDirection();
        }

        private void Move()
        {
            Vector3 currentPosition = transform.position;
            if (shouldMove || !isAttacking)
            {
                // How far away are we from the target?
                float distance = (patrolPoints[currentPoint] - (Vector2)currentPosition).magnitude;

                // If we are closer to our target than our minimum distance...
                if (distance <= stopDistance)
                {
                    // Update to the next target
                    currentPoint = currentPoint + 1;
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
                }
            }

            if (shouldMove)
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

            if (!shouldMove && !isAtWaypointStop)
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
    }
}
