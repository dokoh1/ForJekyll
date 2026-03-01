using UnityEngine;

public class DemoSceneInitial : MonoBehaviour
{
    [SerializeField] private GameObject text;
    private void Start()
    {
        text.SetActive(true);
        GameManager.Instance.CursorVisible();
    }
}