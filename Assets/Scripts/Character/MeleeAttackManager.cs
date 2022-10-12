using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class MeleeAttackManager : MonoBehaviour
    {
        public float defaultForce = 300;
        public float movementTime = .1f;
        private bool meleeAttack;
        private Animator meleeAnimator;
        private Animator animator;
        private CharacterController playerCharacter;

        private void Start()
        {
            animator = GetComponent<Animator>();
            playerCharacter = GetComponent<CharacterController>();
            meleeAnimator = playerCharacter.GetComponentInChildren<MeleeWeapon>().GetComponent<Animator>();
        }

        private void Update()
        {
            CheckInput();
        }
        
        private void CheckInput()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                meleeAttack = true;
            }
            else
            {
                meleeAttack = false;
            }
            
            if (meleeAttack && playerCharacter.IsGrounded())
            {
                animator.SetTrigger("Melee");
                meleeAnimator.SetTrigger("MeleeSwipe");
            }
        }
    }
}
