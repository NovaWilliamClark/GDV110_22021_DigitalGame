/*******************************************************************************************
*
*    File: EnemyBehaviour.cs
*    Purpose: Character Controller Movement and Interaction
*    Author: Joshua Stephens
*    Date: 11/10/2022
*
**********************************************************************************************/

using System.Collections;
using Audio;
using UnityEngine;


// TODO: REFACTOR ENTIRE ENEMY BEHAVIOUR

namespace AI
{
    public class EnemyBehaviour : MonoBehaviour
    {
        public Transform rayCast;
        public LayerMask raycastMask;
        [SerializeField] public float rayCastLength;
        [SerializeField] public float attackDistance;
        [SerializeField] public float moveSpeed;
        [SerializeField] public float atkCooldownTime;
        [SerializeField] public Animator animator;
        
        private RaycastHit2D hit;
        private GameObject target;
        private float distance;
        private bool attackMode;
        private bool inRange;
        private bool inCooldown;
        private static readonly int Melee = Animator.StringToHash("Melee");
        private static readonly int IsMoving = Animator.StringToHash("IsMoving");

        [SerializeField] private AudioClip attackSFX;

        // Update is called once per frame
        void Update()
        {
            if (inRange)
            {
                hit = Physics2D.Raycast(rayCast.position, Vector2.left, rayCastLength, raycastMask);
                //RaycastDebugger();
            }
            else
            {
                animator.SetBool(IsMoving, false);
            }

            if (hit.collider != null)
            {
                EnemyLogic();
            }
            else if (hit.collider == null)
            {
                inRange = false;
            }
        }

        void OnTriggerEnter2D(Collider2D trig)
        {
            if (trig.gameObject.CompareTag("Player"))
            {
                target = trig.gameObject;
                inRange = true;
            }
        }

        void EnemyLogic()
        {
            distance = Vector2.Distance(transform.position, target.transform.position);

            if (distance > attackDistance && !attackMode)
            {
                Move();
            }
            else if (attackDistance >= distance && inCooldown == false)
            {
                Attack();
            }

        }

        void Move()
        {
            animator.SetBool(IsMoving, true);
            var position = transform.position;
            Vector2 targetPosition = new Vector2(target.transform.position.x, position.y);
            position = Vector2.MoveTowards(position, targetPosition, moveSpeed * Time.deltaTime);
            transform.position = position;
        }

        void Attack()
        {
            inCooldown = true;
            attackMode = true;
            animator.SetTrigger(Melee);
            AudioManager.Instance.PlaySound(attackSFX); // TODO: Move to callback driven by animation events
        }
        

        public IEnumerator AttackCooldownReset()
        {
            yield return new WaitForSeconds(atkCooldownTime);
            inCooldown = false;
            attackMode = false;
        }
    }
}
