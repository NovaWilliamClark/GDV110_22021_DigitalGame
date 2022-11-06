using System;
using System.Collections;
using Audio;
using Objects;
using UnityEngine;
using UnityEngine.Rendering.UI;


public class JackInTheBox : MovableObject
{
    [SerializeField] private float dragTime = 3f;
    [SerializeField] private GameObject jack;
    
    private bool isInProgress = false;
    private bool isFinished = false;

    private void Start()
    {
        CharacterController.OnObjectMove += CharacterController_OnObjectMove;
    }

    private void OnDestroy()
    {
        CharacterController.OnObjectMove -= CharacterController_OnObjectMove;
    }

    private void CharacterController_OnObjectMove(bool obj)
    {
        if (!isFinished)
        {
            if (obj && !isInProgress)
            {
                isInProgress = true;
                StartCoroutine(BoxDragRoutine());
            }
            else
            {
                if (!obj && isInProgress)
                {
                    isInProgress = true;
                    StopCoroutine(BoxDragRoutine());
                }
            }
        }
    }

    private IEnumerator BoxDragRoutine()
    {
        yield return new WaitForSeconds(dragTime);
        jack.SetActive(true);
        //FindObjectOfType<CharacterController>().TakeSanityDamage(sanityDamage,false);
        Debug.Log("SURPRISE MOTHERFUCKER!!!");
        isFinished = true;
    }
}