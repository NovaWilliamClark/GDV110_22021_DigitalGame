using System.Collections;
using Audio;
using Objects;
using UnityEngine;

namespace AI
{
    [SerializeField] private GameObject jack;
    [SerializeField] private AudioClip boo;
    [SerializeField] private AudioClip song;
    [SerializeField] private Vector2 destination;
    [SerializeField] private float popSpeed = 1f;
    [SerializeField] private float sanityDamage = 20f;
    [SerializeField] private Animator animator;

    private CharacterController ccontroller;

    protected override void Start()
    {
        base.Start();
        ccontroller = FindObjectOfType<CharacterController>();
    }

        private CharacterController characterController;

        protected override void Start()
        {
            base.Start();
            characterController = FindObjectOfType<CharacterController>();
        }

        protected override void Update()
        {
            base.Update();
        
        /*if (jack.activeInHierarchy)
        {
            jack.transform.position = Vector2.Lerp(jack.transform.position, new Vector2(transform.position.x, destination.y), Time.deltaTime * popSpeed);
        }*/
    }

    protected override void Interact(CharacterController controller)
    {
        DisablePrompt();
        StartCoroutine(JacksBoxyRoutine());
    }

    private IEnumerator JacksBoxyRoutine()
    {
        AudioManager.Instance.PlaySound(song, 1f);
        while (true)
        {
            DisablePrompt();
            StartCoroutine(JacksBoxyRoutine());
        }

    private void SayBoo()
    {
        if (animator != null)
        {
            animator.SetTrigger("Trigger");
            //StartCoroutine(AnimatorRoutine());
            //animator.SetBool("Jumping", false);
        }
        animator.SetBool("Stop", true);
        AudioManager.Instance.PlaySound(boo,1f);
        if (ccontroller != null)
        {
            ccontroller.GetCharacterSanity.DecreaseSanity(sanityDamage, false);
            Debug.Log("Sanity damaged");
        }

        private void SayBoo()
        {
            AudioManager.Instance.PlaySound(boo,1f);
            if (characterController != null)
            {
                characterController.GetCharacterSanity.DecreaseSanity(sanityDamage, false);
                Debug.Log("Sanity damaged");
            }
        
        }
    }

    /*private IEnumerator AnimatorRoutine()
    {
        //yield return new WaitForSeconds(3f);
        
    }*/
    
}