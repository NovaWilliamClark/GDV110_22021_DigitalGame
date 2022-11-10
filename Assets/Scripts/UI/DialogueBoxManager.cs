using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class DialogueBoxManager : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private CanvasGroup currentCanvas;

    private void Awake()
    {
        currentCanvas.alpha = 0;
    }

    public void PrintText(string newText, Vector3 newPosition, float holdDuration, float fadeDuration)
    {
        gameObject.SetActive(true);
        gameObject.transform.position = newPosition;
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = newText;
        FadeIn(fadeDuration);

        StartCoroutine(FadeOutTimer(fadeDuration, holdDuration));
    }

    public void FadeIn(float fadeDuration)
    {
        currentCanvas.DOFade(1, fadeDuration);
    }

    private IEnumerator FadeOutTimer(float fadeDuration, float holdDuration)
    {
        yield return new WaitForSeconds(holdDuration + fadeDuration);
        
        FadeOut(fadeDuration);
    }
    
    private void FadeOut(float fadeDuration)
    {
        currentCanvas.DOFade(0, fadeDuration).OnComplete(()
            =>
        {
            gameObject.SetActive(false);
        });
    }
}
