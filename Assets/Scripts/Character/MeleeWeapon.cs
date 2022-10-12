using System;
using System.Collections;
using AI;
using UnityEngine;

namespace Character
{
    public class MeleeWeapon : MonoBehaviour
    {
        [Header("Defaults")]
        [SerializeField] private int damageAmount = 20;

        private CharacterController character;
        private Rigidbody2D playerRigidBody;
        private MeleeAttackManager meleeAttackManager;
        private Vector2 recoilDirection;
        private bool hasCollided;

        private void Start()
        {
            character = GetComponentInParent<CharacterController>();
            playerRigidBody = GetComponentInParent<Rigidbody2D>();
            meleeAttackManager = GetComponentInParent<MeleeAttackManager>();
        }

        private void FixedUpdate()
        {
            HandleMovement();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<EnemyHealth>())
            {
                HandleCollision(collision.GetComponent<EnemyHealth>());
                Debug.Log("Hit Enemy Health");
            }
        }

        private void HandleCollision(EnemyHealth objHealth)
        {
            if (character.IsGrounded())
            {
                if (character.IsFacingLeft())
                {
                    recoilDirection = Vector2.right;
                }
                else
                {
                    recoilDirection = Vector2.left;
                }
                hasCollided = true;
            }

            objHealth.TakeDamage(damageAmount);
            StartCoroutine(ColliderClear());
        }

        private void HandleMovement()
        {
            if (hasCollided)
            {
                playerRigidBody.AddForce(recoilDirection * meleeAttackManager.defaultForce);
            }
        }

        private IEnumerator ColliderClear()
        {
            yield return new WaitForSeconds(meleeAttackManager.movementTime);
            hasCollided = false;
        }
    }
}
