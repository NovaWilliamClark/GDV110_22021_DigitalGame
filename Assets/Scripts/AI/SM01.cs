using System;
using System.Collections;
using System.Linq;
using Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;


// TODO: AUDIO FX

namespace AI
{
    [RequireComponent(typeof(PersistentObject))]
    public class SM01 : Enemy, ILightResponder
    {
        [Header("Movement")]
        [SerializeField] private float moveVelocity = 10;
        [SerializeField] private float stopDistance = 5;
        [SerializeField] private float maxWaypointStopTime = 3;
        [SerializeField] private Vector2[] patrolPoints;
        [SerializeField] private Vector2 chaseCastSize;
        private RaycastHit2D boxCastHit;
        private Vector3 startPosition;
        private Vector2 direction = new (-1, 1);
        private bool shouldMove = true;
        private bool isAtWaypointStop;
        private bool isPlayerInChaseRange;
        private int currentPoint;

        [Header("Combat")]
        [SerializeField] public float attackDistance;
        [SerializeField] private float attackCooldown = 3;
        private GameObject targetPlayer;
        private SM01AttackCollider attackCollider;
        private bool isAttacking;
        private bool isInAttackCooldown;
        
        [Header("Components")]
        [SerializeField] public Animator animator;
        [SerializeField] private Transform puppet;
        [SerializeField] private LayerMask lightMask;
        private readonly Vector3 flippedScale = new(-1, 1, 1);

        // Start is called before the first frame update
        protected override void Awake()
        {
            base.Awake();
            attackCollider = GetComponentInChildren<SM01AttackCollider>();
            persistentObject = GetComponent<PersistentObject>();
            var lc = FindObjectOfType<LevelController>();
            lc.PlayerSpawned.AddListener(OnPlayerLoaded);
        }

        private void OnPlayerLoaded(CharacterController controller)
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
                case true when !isPlayerInChaseRange:
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
                    
                    if (shouldMove && !isPlayerInChaseRange)
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
                case true when isPlayerInChaseRange:
                {
                    var position = transform.position;
                    Vector2 targetPosition = new Vector2(targetPlayer.transform.position.x, position.y);
                    var distance = (targetPosition - (Vector2)position).magnitude;
                    
                    if (distance <= attackDistance && !isInAttackCooldown)
                    {
                        Attack();
                    }
                    
                    if (!isAttacking)
                    {

                        direction = (targetPosition - (Vector2)position).normalized;
                        position = Vector2.MoveTowards(position, targetPosition, moveVelocity * Time.deltaTime);
                        transform.position = position;
                    
                        // Stop Chasing when Player Exceeds Chase Distance
                        if (distance >= chaseCastSize.x - 0.09f * chaseCastSize.x)
                        {
                            isPlayerInChaseRange = false;
                        }   
                    }

                    break;
                }
            }

            if (!shouldMove && !isAtWaypointStop && !isAttacking)
            {
                StartCoroutine(WayPointStopTime());
            }
        }


        private void Attack()
        {
            shouldMove = false;
            isAttacking = true;
            animator.SetTrigger("Melee");
            isInAttackCooldown = true;
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
            boxCastHit = Physics2D.BoxCast(transform.position + new Vector3(chaseCastSize.x * 0.5f * direction.x, -chaseCastSize.y, 0), chaseCastSize, 0, direction);

            if (!boxCastHit.collider.gameObject.CompareTag("Player")) return;
            if (!isInAttackCooldown)
            {
                animator.SetBool("IsMoving", true);
            }
            isPlayerInChaseRange = true;
            shouldMove = true;
        }

        public IEnumerator AttackCooldownReset()
        {
            yield return new WaitForSeconds(attackCooldown);
            attackCollider.DeactivateCollisionBox();
            isInAttackCooldown = false;
            isAttacking = false;
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
            killed = true;
            
            // TODO: DEATH SFX
            Destroy(gameObject);
            
        }

        public void OnLightExited(float intensity)
        { }

        public void OnLightStay(float intensity)
        { }

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
            Gizmos.DrawWireCube(transform.position + new Vector3(chaseCastSize.x * 0.5f * direction.x, chaseCastSize.y * -0.5f, 0), chaseCastSize);
        }

        public override void SetEnemyState(EnemyLevelState data)
        {
            gameObject.SetActive(data.active);
            enabled = data.behaviourEnabled;
        }
    }
}
