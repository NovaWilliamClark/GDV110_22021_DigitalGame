using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UILogoRandomLetter : MonoBehaviour
{
    [SerializeField] private List<Sprite> letterSprites;
    public Image LetterSprite;

    void Awake()
    {
        LetterSprite = GetComponent<Image>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(SelectRandomSprite());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SelectRandomSprite()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(0.5f, 5f));
            if (letterSprites.Count > 0)
                LetterSprite.sprite = letterSprites[Random.Range(0, letterSprites.Count)];
        }
    }
}