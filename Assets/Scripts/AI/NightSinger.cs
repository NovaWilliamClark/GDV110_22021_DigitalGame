using Core;
using UnityEngine;

public class NightSinger : MonoBehaviour, ILightResponder
{
    [SerializeField] private float sanityDrainMultFar = 1.5f;
    [SerializeField] private float multFarThreshold = 10f;
    [SerializeField] private float sanityDrainMultMid = 2f;
    [SerializeField] private float multMidThreshold = 5f;
    [SerializeField] private float sanityDrainMultNear = 2.5f;
    [SerializeField] private float multNearThreshold = 2f;
    private float currentSanityDrainMult = 1;
    private float currentlossRate = 1;
    
    private enum State
    {
        Idle,
        Singing,
        Enraged
    }

    private bool isSinging = false;
    private bool isEnraged = false;
    private State currentState = State.Idle;
    private CharacterController player;

    private void Start()
    {
        player = FindObjectOfType<CharacterController>();
    }

    private void Update()
    {
        if (currentState == State.Idle)
        {
            //sleepy stuff
        }
        else if (currentState == State.Singing)
        {
            if (!isSinging)
            {
                Sing();
            }
        }
        else if (currentState == State.Enraged)
        {
            if (!isEnraged)
            {
                isEnraged = true;
                BeEnragedLOL();
            }
        }
        
        UpdateSanityLossMult();
        UpdateSanityLoss();
        
        
    }

    private void UpdateSanityLoss()
    {
        float lossRate = player.GetCharacterSanity.GetData.sanityLossRate * currentSanityDrainMult;
        player.GetCharacterSanity.AdjustDecreaseRate(lossRate);
    }

    private void BeEnragedLOL()
    {
        //Ragey stuff
    }

    private void UpdateSanityLossMult()
    {
        float dir = Vector2.Distance(transform.position, player.transform.position);
        if (dir > multFarThreshold)
        {
            if (isSinging)
            {
                currentSanityDrainMult = 1;
                UpdateSanityLoss();
                StopSinging();
            }
        }
        else if (dir <= multFarThreshold && dir > multMidThreshold)
        {
            currentSanityDrainMult = sanityDrainMultFar;
            UpdateSanityLoss();
        }
        else if (dir <= multMidThreshold && dir > multNearThreshold)
        {
            currentSanityDrainMult = sanityDrainMultMid;
            UpdateSanityLoss();
        }
        else
        {
            currentSanityDrainMult = sanityDrainMultNear;
            UpdateSanityLoss();
        }
    }

    private void Sing()
    {
        isSinging = true;
        //start sound
    }

    private void StopSinging()
    {
        //stop sound
        isSinging = false;
        currentState = State.Idle;
    }

    public void OnLightEntered(float intensity)
    {
        currentState = State.Enraged;
    }

    public void OnLightExited(float intensity)
    {
        currentState = State.Singing;
    }

    public void OnLightStay(float intensity)
    {
        //Is pissed
    }
}