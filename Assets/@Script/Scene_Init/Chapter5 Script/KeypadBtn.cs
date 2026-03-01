using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class KeypadBtn : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Button keypadBtn;
    [SerializeField] private int keypadNum;
    
    private Sequence delaySequence;

    private void Awake()
    {
        buttonImage ??= GetComponent<Image>();
        keypadBtn ??= GetComponent<Button>();
        keypadBtn.onClick.AddListener(() => InteractButton(keypadNum));
    }

    private void InteractButton(int num)
    {
        delaySequence?.Kill();
        delaySequence = DOTween.Sequence();
        
        
        delaySequence.AppendInterval(1f);
    }
    
    public void ButtonInteractToggle(bool value) { keypadBtn.interactable = value; }
}