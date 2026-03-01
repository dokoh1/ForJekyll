using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextMeshSave : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI proUGUI;
    [SerializeField] private int slotNum;
    [SerializeField] private Image image;
    private void OnEnable()
    {
        if (proUGUI == null) proUGUI = GetComponent<TextMeshProUGUI>();
        
        DataToSave data = DataManager.Instance.sceneDataManager.DataLoad(slotNum);
        
        if (data != null)
        {
           proUGUI.text = data.saveTxt;
           image.sprite = DataManager.Instance.sceneDataManager.sceneImages[(SceneEnum)data.SceneEnum];
        }
        else
        {
            proUGUI.text = "";
        }
    }

    public void PushData()
    {
        DataManager.Instance.sceneDataManager.nowTextMeshPro = proUGUI;
        DataManager.Instance.sceneDataManager.nowSaveDataNum = slotNum;
        DataManager.Instance.sceneDataManager.nowLoadDataNum = slotNum;
    }
}
