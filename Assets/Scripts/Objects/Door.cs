using System.Collections;
using System.Collections.Generic;
using Audio;
using UnityEngine;
using UnityEngine.Events;

public class Door : InteractionPoint
{
    private Animator animator;
    
    [Header("Scene Transition")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private int spawnPointIndex;
    
    private bool interacted;

    [Header("Animation")]
    public float delayAfterAnimation = 1f;
    private static readonly int Open = Animator.StringToHash("Open");

    
    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    protected override void Interact(CharacterController cc)
    {
        if (!interacted)
        {
            interacted = true;
            cc.SetPersistentData();
            if (animator)
                animator.SetTrigger(Open);

            AudioManager.Instance.PlaySound(useSfx, volume);
            cc.ToggleMovement(false);

            var sfxLength = useSfx ? useSfx.length : 0.5f;
            
            StartCoroutine(WaitForThen(() =>
            {
                TransitionManager.Instance.SetSpawnIndex(spawnPointIndex);
                UIHelpers.Instance.Fader.Fade(1f, sfxLength + 0.1f, () =>
                {
                    TransitionManager.Instance.LoadScene(sceneToLoad);
                });
            }));
        }
    }

    public IEnumerator WaitForThen(UnityAction callback = null)
    {
        yield return new WaitForSeconds(delayAfterAnimation);
        callback?.Invoke();
    }
}