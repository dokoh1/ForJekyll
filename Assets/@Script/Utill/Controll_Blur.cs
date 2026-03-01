using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Controll_Blur : MonoBehaviour
{
    public Volume globalVolume;

    [SerializeField] private DepthOfField dof;
    [SerializeField] private Vignette vignette;

    [SerializeField] private bool toggleDof;

    [SerializeField] private float foucsDistance; // ����
    [SerializeField] private float minValue = 0.5f; // �ּҰ�
    [SerializeField] private float maxValue = 1f; // �ִ밪
    [SerializeField] private float duration = 1f; // 0.5 -> 1�� �ö󰡴� �� �ɸ��� �ð�
    [SerializeField] private float finalValue = 3f; // ������ ��

    [SerializeField] private Image eyeImage;

    public IEnumerator CallValue(int num)
    {
        yield return new WaitForSeconds(3f);
        if (globalVolume.profile.TryGet(out dof)) 
        {
            dof.active = true;
        } 
        else yield break;

        if (globalVolume.profile.TryGet(out vignette)) 
        { 
            vignette.active = true;
        } 
        else yield break;

        for (int i = 0; i < num; i++)
        {
            yield return StartCoroutine(BlueChangeValue(maxValue, minValue, duration, false));

            yield return StartCoroutine(BlueChangeValue(minValue, maxValue, duration, false));
        }
        yield return StartCoroutine(BlueChangeValue(minValue, finalValue, duration, true));
        
        vignette.active = false;
        dof.mode.value = DepthOfFieldMode.Off;
    }
    IEnumerator BlueChangeValue(float start, float end, float time, bool wakeUp)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            Color color = eyeImage.color;

            foucsDistance = Mathf.Lerp(start, end, elapsedTime / time);

            elapsedTime += Time.deltaTime;

            dof.focusDistance.value = foucsDistance;

            if (!wakeUp)
            {
                color.a = Mathf.Lerp(start, end, elapsedTime / time);
            }
            else
            {
                color.a = Mathf.Lerp(1, 0, elapsedTime / time);
                float v = Mathf.Lerp(vignette.intensity.value, 0, elapsedTime / 3);
                vignette.intensity.value = v;
            }

            eyeImage.color = color;

            yield return null;
        }
        foucsDistance = end;
    }
}
