using Marker;
using Sirenix.OdinInspector;
using UnityEngine;

public enum PlayerPosition
{
    B1,
    B2,
}
public class FirePlugManager : MonoBehaviour
{
    [TabGroup("Chapter2_3/Tabs", "Script")]
    public NoiseFirePlug[] B1_FirePlug;
    
    [TabGroup("Chapter2_3/Tabs", "Script")]
    public ColliderHandler PlayerPositionTrigger;
    
    [TabGroup("Chapter2_3/Tabs", "Script")]
    public NoiseFirePlug[] B2_FirePlug;
    
    [BoxGroup("Chapter2_3")] [TabGroup("Chapter2_3/Tabs", "QuestTarget")]
    public QuestTarget[] B1_FirePlugTarget;
    
    [BoxGroup("Chapter2_3")] [TabGroup("Chapter2_3/Tabs", "QuestTarget")]
    public QuestTarget[] B2_FirePlugTarget;

    private PlayerPosition _playerPosition = PlayerPosition.B1;
    
    public void Initialize()
    {
        for (int i = 0; i < B1_FirePlugTarget.Length; i++)
        {
            UIManager.Instance.objective.SetMarkerByKeyAt(i + 1, "CreateNoise");
            // UIManager.Instance.objective.FollowAt(i + 1, B1_FirePlugTarget[i], 1f);
        }
    }

    private void CheckFloorTransition()
    {
        if (_playerPosition == PlayerPosition.B1)
        {
            _playerPosition = PlayerPosition.B2;
        }
        else if (_playerPosition == PlayerPosition.B2)
        {
            _playerPosition = PlayerPosition.B1;
        }
    }
   
}
