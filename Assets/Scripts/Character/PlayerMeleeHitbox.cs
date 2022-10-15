using System.Collections;
using System.Collections.Generic;
using Character;
using UnityEngine;

public class PlayerMeleeHitbox : MonoBehaviour
{
    private MeleeWeapon playerWeapon;
    
    private void Start()
    {
        playerWeapon = GetComponentInParent<CharacterController>().GetComponentInChildren<MeleeWeapon>();
    }
    
    private void EnableDamageHitbox()
    {
        playerWeapon._animator.SetTrigger("MeleeSwipe");
        StartCoroutine(playerWeapon.AttackCooldownReset());
    }
}
