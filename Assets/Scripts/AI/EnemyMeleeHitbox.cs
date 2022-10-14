using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeHitbox : MonoBehaviour
{
    private EnemyBehaviour enemy;
    
    private void Start()
    {
        enemy = GetComponentInParent<EnemyBehaviour>();
    }
    
    private void EnableDamageHitbox()
    {
        enemy.weaponAnimator.SetTrigger("MeleeSwipe");
        StartCoroutine(enemy.AttackCooldownReset());
    }
}
