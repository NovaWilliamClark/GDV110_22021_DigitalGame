using Character;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SanityChanger : MonoBehaviour
{
    [SerializeField] private float sanityChangeRate; // Use negative values to slow sanity change
    [SerializeField] private float sanityDamage = 10f;
    [SerializeField] private bool dealDamageAlso = false;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.TryGetComponent<CharacterSanity>(out var player))
        {
            player.AdjustDecreaseRate(sanityChangeRate);
            if (dealDamageAlso)
            {
                player.DecreaseSanity(sanityDamage, false);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (TryGetComponent<CharacterSanity>(out var player))
        {
            player.AdjustDecreaseRate(0, true);
        }
    }
}