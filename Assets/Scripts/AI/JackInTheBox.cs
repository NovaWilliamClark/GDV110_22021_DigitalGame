using System.Collections;
using Audio;
using Objects;
using UnityEngine;


public class JackInTheBox : MovableObject
{
    [SerializeField] private GameObject jack;
    [SerializeField] private AudioClip boo;
    [SerializeField] private AudioClip song;
    [SerializeField] private Vector2 destination;
    [SerializeField] private float popSpeed = 1f;
    [SerializeField] private float sanityDamage = 20f;

    private CharacterController cc;

    protected override void Start()
    {
        base.Start();
        cc = FindObjectOfType<CharacterController>();
    }

    protected override void Update()
    {
        base.Update();
        
        if (jack.activeInHierarchy)
        {
            jack.transform.position = Vector2.Lerp(jack.transform.position, new Vector2(transform.position.x, destination.y), Time.deltaTime * popSpeed);
        }
    }

    protected override void Interact(CharacterController cc)
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
        if (cc != null)
        {
            cc.GetCharacterSanity.DecreaseSanity(sanityDamage, false);
            Debug.Log("Sanity damaged");
        }
        
    }
}