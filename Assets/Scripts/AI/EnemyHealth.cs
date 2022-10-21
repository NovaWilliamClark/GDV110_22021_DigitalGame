/*******************************************************************************************
*
*    File: EnemyHealth.cs
*    Purpose: Handles Enemy Health and Damage
*    Author: William Clark
*    Date: 10/10/2022
*
**********************************************************************************************/

using System;
using System.Collections;
using UnityEngine;

namespace AI
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("Defaults")]
        [SerializeField] private bool isDamageable = true;
        [SerializeField] private int healthAmount = 100;
        [SerializeField] private float invulnerabilityTime = .2f;
    
        private bool hasBeenHit;
        private bool IsDead;
        private int currentHealth;

        // Start is called before the first frame update
        void Start()
        {
            currentHealth = healthAmount;
        }

        public void TakeDamage(int damage)
        {
            if (isDamageable && !hasBeenHit && currentHealth > 0)
            {
                hasBeenHit = true;
                currentHealth -= damage;
                Debug.Log("No Hit to Enemy Health");
                if (currentHealth <= 0)
                {
                    // Death Call
                    Destroy(gameObject);
                }
                else
                {
                    StartCoroutine(TurnOffHitStun());
                }
            }
        }

        private IEnumerator TurnOffHitStun()
        {
            yield return new WaitForSeconds(invulnerabilityTime);
            hasBeenHit = false;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.GetComponent<CharacterController>())
            {
            //    Debug.Log("Player hit");
            }
            
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Hitting player");
            }
        }
    }
}
