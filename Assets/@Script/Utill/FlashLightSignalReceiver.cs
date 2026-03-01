using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class FlashLightSignalReceiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        if (notification is SignalEmitter signal)
        {
            // 시그널 이름 확인
            if (signal.asset.name == "FlashlightHideSignal")
            {
                // 플래시 손 내려가는 애니메이션 트리거 실행
                var player = GameManager.Instance.Player;
                if (player != null && player.Flash != null)
                {
                    player.Flash.Animator.SetTrigger(player.Flash.FlashlightHideTrigger);
                }
            }
        }
    }
}
