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
        public GameObject meleeSprite;
        private MeleeWeapon _meleeWeapon;

        private void Start()
        {
            playerCharacter = GetComponent<CharacterController>();
            animator = playerCharacter.animator;
            _meleeWeapon = GetComponentInChildren<MeleeWeapon>();
            
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
                _meleeWeapon.Attack();
            }
        }
    }
}
