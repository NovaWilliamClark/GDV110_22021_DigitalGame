using UnityEngine;

namespace AI
{
    public class EnemyMeleeHitbox : MonoBehaviour
    {
        private EnemyBehaviour enemy;
    
        private void Start()
        {
            enemy = GetComponentInParent<EnemyBehaviour>();
        }
    
        private void EnableDamageHitbox()
        {
            StartCoroutine(enemy.AttackCooldownReset());
        }
    }
}
