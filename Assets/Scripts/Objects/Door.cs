using System.Collections;
using System.Collections.Generic;
using Audio;
using Objects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Door : SceneTransition
{
    private Animator animator;
    private bool interacted;

    [Header("Animation")]
    public float delayAfterAnimation = 1f;
    private static readonly int Open = Animator.StringToHash("Open");

    [SerializeField] bool useAnimation = true;
    
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
            if (animator && useAnimation)
                animator.SetTrigger(Open);

            AudioManager.Instance.PlaySound(useSfx, volume);
            cc.ToggleMovement(false);

            var sfxLength = useSfx ? useSfx.length : 0.5f;
            
            StartCoroutine(WaitForThen(() =>
            {
                var tmi = TransitionManager.Instance;
                tmi.previousScene = SceneManager.GetActiveScene().name;
                //tmi.transitionInteractable = persistentObject.Id;
                tmi.isChangingScenes = true;
                Interacted?.Invoke(this, new InteractionState(persistentObject.Id){interacted = true});
                UIHelpers.Instance.Fader.Fade(1f, sfxLength + 0.1f, () =>
                {
                    TransitionManager.Instance.LoadScene(sceneToLoad.sceneName);
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