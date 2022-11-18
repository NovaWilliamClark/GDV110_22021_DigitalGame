using System.Collections;
using Audio;
using Objects;
using UnityEngine;

namespace AI
{
    public class JackInTheBox : MovableObject
    {
        [SerializeField] private GameObject jack;
        [SerializeField] private AudioClip boo;
        [SerializeField] private AudioClip song;
        [SerializeField] private Vector2 destination;
        [SerializeField] private float popSpeed = 1f;
        [SerializeField] private float sanityDamage = 20f;

        private CharacterController characterController;

        protected override void Start()
        {
            base.Start();
            characterController = FindObjectOfType<CharacterController>();
        }

        protected override void Update()
        {
            base.Update();
        
            if (jack.activeInHierarchy)
            {
                jack.transform.position = Vector2.Lerp(jack.transform.position, new Vector2(transform.position.x, destination.y), Time.deltaTime * popSpeed);
            }
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
                yield return new WaitForSeconds(song.length - 5);
                break;
            }
            jack.SetActive(true);
            SayBoo();
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
}