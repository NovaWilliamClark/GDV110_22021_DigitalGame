using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void TakeDamage(int damage)
    {
        if (damage > 0 && !hasBeenHit && currentHealth > 0)
        {
            hasBeenHit = true;
            currentHealth -= damage;
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
}
