using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;

public class UIBatteryIndicator : MonoBehaviour
{
    // battery indicator needs to
    [Header("Battery Status Colour")]
    [SerializeField] private Color criticalColour;
    [SerializeField] private Color normalColour;
    [SerializeField] private float criticalValue = 0.2f;
    
    [Header("Component references")]
    [SerializeField] private CanvasGroup batteryCanvasGroup;
    [SerializeField] private Image fillableBg;
    [SerializeField] private Image fillable;
    [SerializeField] private Image background;
    [SerializeField] private GameObject icon;
    [SerializeField] private Image iconOn;
    [SerializeField] private Image iconOff;

    public PlayerData_SO playerData;

    private Sequence _introSequence;

    private bool IsCritical => fillable.fillAmount <= criticalValue;

    //private float curTime = 1f;
    private Sequence criticalSeq;

    public void Init()
    {
        batteryCanvasGroup.alpha = 0;
        background.GetComponent<RectTransform>().DOScale(0f, 0f);
        icon.GetComponent<RectTransform>().DOScale(0f, 0f);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _introSequence = DOTween.Sequence();
        _introSequence
            .Append(background.GetComponent<RectTransform>().DOScale(1f, .5f))
            .Append(icon.GetComponent<RectTransform>().DOScale(1f, .5f))
            .Append(batteryCanvasGroup.DOFade(1f, 1f))
            .SetAutoKill(false);

        _introSequence.Play();
        
        fillable.fillAmount = 0f;
        FlashCritical(4, fillableBg, FinishSetup);
    }

    void FinishSetup()
    {
        var curVal = playerData.CurrentBattery / playerData.MaxBattery;
        fillable.fillAmount = curVal;
        playerData.BatteryValueChanged.AddListener(OnBatteryValueChanged);
    }

    private void Update()
    {
        if (criticalSeq.IsPlaying()) return;
        fillable.color = fillable.fillAmount <= criticalValue ? criticalColour : normalColour;
    }

    private void FlashCritical(int number, Image target, UnityAction complete = null)
    {
        var oldColour = !IsCritical ? criticalColour : target.color;
        var toColour = target.color != criticalColour ? criticalColour : Color.black;
        target.color = criticalColour;
        criticalSeq = DOTween.Sequence();
        criticalSeq.Append(target.DOColor(toColour, .2f));
        criticalSeq.Append(target.DOColor(oldColour, .2f));
        criticalSeq.SetLoops(number);
        criticalSeq.SetAutoKill(false);
        criticalSeq.OnComplete(() =>
        {
            complete?.Invoke();
        });
        criticalSeq.Play();
    }

    // public void SetValue(float value)
    // {
    //     playerData.CurrentBattery = value;
    // }
    //
    // public void Reduce(float tick)
    // {
    //     curTime = tick/60f;
    //     StartCoroutine(Degen(tick));
    // }
    //
    // private IEnumerator Degen(float tick)
    // {
    //     while (true)
    //     {
    //         if (playerData.CurrentBattery <= 0f)
    //         {
    //             break;
    //         }
    //
    //         curTime -= Time.deltaTime;
    //         if (curTime <= 0)
    //         {
    //             curTime = 0f;
    //             break;
    //         }
    //         playerData.CurrentBattery = Mathf.Lerp(0f,100,curTime/(tick/60));
    //         yield return null;
    //     }
    // }

    private void OnFlashlightToggled(bool enabled)
    {
        iconOff.gameObject.SetActive(!enabled);
        iconOn.gameObject.SetActive(enabled);
    }
    
    private void OnBatteryValueChanged(float value)
    {
        var val = value / playerData.MaxBattery;
        fillable.fillAmount = val;
        if (val > criticalValue) return;
        if (!criticalSeq.IsPlaying())
        {
            FlashCritical(2, fillable);
        }
    }
}
