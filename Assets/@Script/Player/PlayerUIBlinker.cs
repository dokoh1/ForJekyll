using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIBlinker : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private float blinkInterval = 0.4f;
    [SerializeField] private float blinkDuration = 5f;

    public bool IsBlinking => _blinkCoroutine != null;

    private Coroutine _blinkCoroutine;
    private WaitForSeconds _cachedWait;
    //private WaitForSeconds _cachedWait = new WaitForSeconds(0.5f);

    //private void OnEnable()
    //{
    //    _cachedWait = new WaitForSeconds(blinkInterval);

    //    //StartBlink();
    //}

    private void Start()
    {
        _cachedWait = new WaitForSeconds(blinkInterval);
    }

    public void StartBlink()
    {
        if (_blinkCoroutine != null)
            StopCoroutine(_blinkCoroutine);

        _blinkCoroutine = StartCoroutine(BlinkCoroutine());
    }

    private IEnumerator BlinkCoroutine()
    {
        float elapsed = 0f;
        bool isVisible = true;

        while (elapsed < blinkDuration)
        {
            targetImage.enabled = isVisible;
            isVisible = !isVisible;

            yield return _cachedWait;
            elapsed += blinkInterval;
        }

        targetImage.enabled = false;
        _blinkCoroutine = null;
    }

    public void StopBlink()
    {
        if (_blinkCoroutine != null)
        {
            StopCoroutine(_blinkCoroutine);
            _blinkCoroutine = null;      
        }

        if (targetImage.enabled)
            targetImage.enabled = false;   
    }
}