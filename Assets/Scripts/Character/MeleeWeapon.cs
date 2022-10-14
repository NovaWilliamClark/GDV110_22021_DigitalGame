using System.Collections;
using AI;
using UnityEngine;

namespace Character
{
    public class MeleeWeapon : MonoBehaviour
    {
        [Header("Defaults")]
        [SerializeField] private int damageAmount = 20;
        [SerializeField] private float atkCooldownTime = 0.3f;
        
        [SerializeField] private float defaultForce = 250.0f;
        [SerializeField] private float movementTime = 0.1f;

        private CharacterController character;
        private Rigidbody2D playerRigidBody;
        private Vector2 recoilDirection;
        private bool hasCollided;
        public Animator _animator;
        [SerializeField] private GameObject weaponSprite;
        private bool _attacking;
        private bool inCooldown;
        private AnimatorClipInfo[] _clipInfo;

        private void Start()
        {
            character = GetComponentInParent<CharacterController>();
            playerRigidBody = GetComponentInParent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
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
                playerRigidBody.AddForce(recoilDirection * defaultForce);
            }
        }

        private IEnumerator ColliderClear()
        {
            yield return new WaitForSeconds(movementTime);
            hasCollided = false;
        }

        public void Attack()
        {
            if (!inCooldown && ! _attacking)
            {
                weaponSprite.SetActive(true);
                character.animator.SetTrigger("Melee");
                _attacking = true;
            }
        }

        public IEnumerator AttackCooldownReset()
        {
            yield return new WaitForSeconds(atkCooldownTime);
            inCooldown = false;
            _attacking = false;
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
