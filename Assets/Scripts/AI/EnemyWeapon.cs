/*******************************************************************************************
*
*    File: EnemyWeapon.cs
*    Purpose: Enemy weapon management
*    Author: Joshua Stephens
*    Date: 12/10/2022
*
**********************************************************************************************/

using UnityEngine;

namespace AI
{
    public class EnemyWeapon : MonoBehaviour
    {
        [Header("Defaults")]
        [SerializeField] private int damageAmount = 20;
        
        private bool hasCollided;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                HandleCollision(collision.GetComponent<CharacterController>());
                Debug.Log("Hit Player");
            }
        }
        
        private void HandleCollision(CharacterController objController)
        {
            objController.TakeSanityDamage(damageAmount);
        }
    }
}
