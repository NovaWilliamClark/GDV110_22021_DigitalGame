using System.Collections;
using Core;
using UnityEngine;

public class NightSinger : MonoBehaviour, ILightResponder
{
    [SerializeField] private float sanityDamageInterval = 1f;
    [SerializeField] private float sanityDamageAmount = 5f;
    
    private enum State
    {
        Sleeping,
        Singing
    }

    private bool isSinging = false;
    private State currentState = State.Sleeping;
    private CharacterController player;

    private void Start()
    {
        player = FindObjectOfType<CharacterController>();
    }

    private void Update()
    {
        if (currentState == State.Sleeping)
        {
            isSinging = false;
            //sleepy stuff
        }
        else if (currentState == State.Singing)
        {
            if (!isSinging)
            {
                isSinging = true;
                Sing();
            }
        }
    }

    private void Sing()
    {
        //start sound
        StartCoroutine(SingRoutine());
    }

    private IEnumerator SingRoutine()
    {
        yield return new WaitForSeconds(sanityDamageInterval);
        if (player != null)
        {
            player.GetCharacterSanity.DecreaseSanity(sanityDamageAmount,false);
            StartCoroutine(SingRoutine());
        }
    }

    private void StopSinging()
    {
        //stop sound
        StopCoroutine(SingRoutine());
        isSinging = false;
    }

    public void OnLightEntered(float intensity)
    {
        currentState = State.Singing;
    }

    public void OnLightExited(float intensity)
    {
        StopSinging();
        currentState = State.Sleeping;
    }

    public void OnLightStay(float intensity)
    {
        
    }
}