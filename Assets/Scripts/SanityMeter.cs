using UnityEngine;
using UnityEngine.UI;

public class SanityMeter : MonoBehaviour
{
    [SerializeField] private Slider m_SanityMeter;
    private bool m_DecreaseSlider = false;
    private Player m_Player;

    private void Awake()
    {
        m_Player = FindObjectOfType<Player>();
    }

    private void Start()
    {
        m_DecreaseSlider = true;
    }

    private void Update()
    {
        if (!m_DecreaseSlider)
        {
            return;
        }

        if (m_SanityMeter.value <= 0f)
        {
            return;
        }
        
        m_SanityMeter.value = m_Player.getSanity;
    }
}