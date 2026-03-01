using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class NotebookAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private GameObject openPrefab;
    [SerializeField] private GameObject closePrefab;
    [SerializeField] private GameObject switchPrefab;
    [SerializeField] private GameObject switchPrefab2;
    [SerializeField] private GameObject[] karmaBloodAnimations;

    [SerializeField] private Transform animationParent;


    public void PlayOpen(System.Action onComplete)
    {
        PlayAnimation(openPrefab, onComplete);
    }

    public void PlayClose(System.Action onComplete)
    {
        PlayAnimation(closePrefab, onComplete);
    }

    public void PlaySwitch(string tab, System.Action onComplete)
    {
        GameObject prefabToPlay;

        switch (tab)
        {
            case "Map":
                prefabToPlay = switchPrefab2;
                break;
            case "Mission":
                prefabToPlay = switchPrefab;
                break;
            default:
                prefabToPlay = switchPrefab;
                break;
        }

        PlayAnimation(prefabToPlay, onComplete);
    }

    public void PlayKarma(int phase, System.Action onComplete)
    {
        PlayAnimation(karmaBloodAnimations[phase - 1], onComplete);
    }

    private void PlayAnimation(GameObject prefab, System.Action onComplete)
    {
        prefab.SetActive(true);

        DOTweenAnimation tweenAnim = prefab.GetComponent<DOTweenAnimation>();

        if (tweenAnim != null)
        {
            tweenAnim.CreateTween();
            tweenAnim.tween.SetUpdate(true);

            tweenAnim.tween.OnComplete(() =>
            {
                onComplete?.Invoke();
                prefab.SetActive(false);
            });

            tweenAnim.DOPlay();
        }
    }
}
