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
        private Animator _animator;
        [SerializeField] private GameObject weaponSprite;
        public AnimationClip meleeAnimation;
        private bool _attacking;
        private AnimatorClipInfo[] _clipInfo;

        private void Start()
        {
            character = GetComponentInParent<CharacterController>();
            playerRigidBody = GetComponentInParent<Rigidbody2D>();
            meleeAttackManager = GetComponentInParent<MeleeAttackManager>();
            _animator = GetComponent<Animator>();
        }

        private void FixedUpdate()
        {
            HandleMovement();
            _clipInfo = _animator.GetCurrentAnimatorClipInfo(0);
            // if (_attacking && _clipInfo[0].clip.name != meleeAnimation.name)
            // {
            //     _attacking = false;
            //     weaponSprite.SetActive(false);
            // }
        
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

        public void Attack()
        {
            weaponSprite.SetActive(true);
            character.animator.SetTrigger("Melee");
            _attacking = true;
            _animator.SetTrigger("MeleeSwipe");
        }
        
        public void OnAttackFinished()
        {
            StartCoroutine(HideSpriteDelay());
        }

        private IEnumerator HideSpriteDelay()
        {
            yield return new WaitForSeconds(1f);
            weaponSprite.SetActive(false);
        }
    }
}
