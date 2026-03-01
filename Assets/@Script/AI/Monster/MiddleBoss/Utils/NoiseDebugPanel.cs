using System.Text;
using MBT;
using TMPro;
using UnityEngine;

public class NoiseDebugPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private NoiseDetectionGaugeManager gaugeManager;
    [SerializeField] private Blackboard midBossBB;
    [SerializeField] private TextMeshProUGUI text; // TMP 텍스트 사용

    // BB 캐시
    private Variable<float> _bbDetectionGauge;
    private Variable<int>  _bbCurState;
    private Variable<bool>  _bbHasNoiseTarget;
    private Variable<Vector3> _bbChargeTargetPos;

    private void Awake()
    {
        if (midBossBB != null)
        {
            // BBKeys 활용해서 안전하게 변수 가져오기
            _bbDetectionGauge  = midBossBB.GetVariable<Variable<float>>(BBKeys.MidBoss.DetectionGauge);
            _bbCurState       = midBossBB.GetVariable<Variable<int>>(BBKeys.MidBoss.CurState);
            _bbHasNoiseTarget  = midBossBB.GetVariable<Variable<bool>>(BBKeys.MidBoss.HasNoiseTarget);
            _bbChargeTargetPos = midBossBB.GetVariable<Variable<Vector3>>(BBKeys.MidBoss.ChargeTargetPos);
        }
    }

    private void Update()
    {
        if (gaugeManager == null || text == null) return;

        var sb = new StringBuilder();

        // ─ GaugeManager 쪽 정보 ─
        sb.AppendLine("<b>[GaugeManager]</b>");
        sb.AppendLine($"Gauge:        {gaugeManager.CurrentGauge:F1}");
        sb.AppendLine($"Player Δ/s:   {gaugeManager.DebugPlayerDeltaPerSec:F1}");
        sb.AppendLine($"Timer Count:  {gaugeManager.DebugActiveTimerCount}");
        sb.AppendLine($"Gain Mult:    {gaugeManager.DebugGainMultiplier:F2}");
        sb.AppendLine($"BlockIncrease:{gaugeManager.DebugBlockIncrease}");
        sb.AppendLine();

        // ─ Blackboard 쪽 정보 ─
        if (midBossBB != null)
        {
            sb.AppendLine("<b>[Blackboard]</b>");
            sb.AppendLine($"BB Gauge:     {_bbDetectionGauge.Value:F1}");
            sb.AppendLine($"curState:   {_bbCurState.Value}");
            sb.AppendLine($"hasNoiseTarget: {_bbHasNoiseTarget.Value}");
            sb.AppendLine($"noisePos:       {_bbChargeTargetPos.Value}");

        }

        text.text = sb.ToString();
    }
}