using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MidBossEarType))]
public class MidBossEarTypeEditor : Editor
{
    public override bool RequiresConstantRepaint() => Application.isPlaying;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (!Application.isPlaying)
            return;

        var boss = (MidBossEarType)target;
        var gauge = NoiseDetectionGaugeManager.Instance;

        EditorGUILayout.Space(10);

        // ── Header ──
        var headerStyle = new GUIStyle(EditorStyles.foldoutHeader)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold
        };
        EditorGUILayout.LabelField("MidBoss Runtime Debug", headerStyle);
        EditorGUILayout.Space(4);

        // ── State ──
        DrawStateSection(boss);

        EditorGUILayout.Space(6);

        // ── Noise Gauge ──
        if (gauge != null)
            DrawNoiseGaugeSection(gauge);

        EditorGUILayout.Space(6);

        // ── Charge ──
        DrawChargeSection(boss);

        EditorGUILayout.Space(6);

        // ── Combo ──
        DrawComboSection(boss);
    }

    private void DrawStateSection(MidBossEarType boss)
    {
        EditorGUILayout.BeginVertical("box");

        var state = boss.CurrentState;

        // 상태별 색상
        Color stateColor = state switch
        {
            MidBossState.Idle   => Color.gray,
            MidBossState.Detect => Color.yellow,
            MidBossState.Rage   => new Color(1f, 0.5f, 0f),  // 주황
            MidBossState.Charge => Color.red,
            MidBossState.Stun   => new Color(0.3f, 0.5f, 1f), // 파랑
            _                   => Color.white
        };

        var prevBg = GUI.backgroundColor;
        GUI.backgroundColor = stateColor;
        EditorGUILayout.BeginHorizontal("button");
        GUI.backgroundColor = prevBg;

        var stateLabelStyle = new GUIStyle(EditorStyles.boldLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 12
        };
        EditorGUILayout.LabelField($"State: {state}", stateLabelStyle);
        EditorGUILayout.EndHorizontal();

        // Enraged
        DrawBoolField("Enraged", boss.IsEnraged);

        // Stun Timer
        EditorGUILayout.LabelField("Stun Timer", $"{boss.StunTimer:F1}s");

        // Wall Hits
        int wallDeathCount = boss.MidBossData != null ? boss.MidBossData.Data.wallDeathCount : 5;
        EditorGUILayout.LabelField("Wall Hits", $"{boss.WallHitCount} / {wallDeathCount}");

        EditorGUILayout.EndVertical();
    }

    private void DrawNoiseGaugeSection(NoiseDetectionGaugeManager gauge)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Noise Gauge", EditorStyles.boldLabel);

        // 프로그레스 바 (색상)
        float current = gauge.CurrentGauge;
        float ratio = Mathf.Clamp01(current / 100f);

        Color barColor;
        if (current <= 30f)
            barColor = Color.Lerp(Color.green, Color.yellow, current / 30f);
        else if (current <= 50f)
            barColor = Color.Lerp(Color.yellow, new Color(1f, 0.5f, 0f), (current - 30f) / 20f);
        else
            barColor = Color.Lerp(new Color(1f, 0.5f, 0f), Color.red, Mathf.Clamp01((current - 50f) / 50f));

        Rect rect = GUILayoutUtility.GetRect(18, 22, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

        Rect filled = new Rect(rect.x, rect.y, rect.width * ratio, rect.height);
        EditorGUI.DrawRect(filled, barColor);

        var barLabelStyle = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white },
            fontStyle = FontStyle.Bold
        };
        GUI.Label(rect, $"{current:F1} / 100", barLabelStyle);

        // Debug 값들
        EditorGUILayout.LabelField("Player \u0394/s", $"{gauge.DebugPlayerDeltaPerSec:+0.0;-0.0;0.0}");
        EditorGUILayout.LabelField("Timer Sources", gauge.DebugActiveTimerCount.ToString());
        EditorGUILayout.LabelField("Gain Multiplier", $"{gauge.DebugGainMultiplier:F1}x");
        DrawBoolField("Increase Blocked", gauge.DebugBlockIncrease);

        EditorGUILayout.EndVertical();
    }

    private void DrawChargeSection(MidBossEarType boss)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Charge", EditorStyles.boldLabel);

        DrawBoolField("Charging", boss.IsCharging);
        EditorGUILayout.LabelField("Start", boss.ChargeStartPos.ToString("F1"));
        EditorGUILayout.LabelField("Hit Radius", boss.ChargePlayerHitRadius.ToString("F1"));

        EditorGUILayout.EndVertical();
    }

    private void DrawComboSection(MidBossEarType boss)
    {
        var combo = boss.ComboSystem;
        if (combo == null)
            return;

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Combo", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        DrawBoolField("Active", combo.IsActive);
        DrawBoolField("Pending", combo.IsPending);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Progress", $"{combo.CurrentIndex} / {combo.TargetCount}");

        // 타겟 목록
        if (combo.Targets != null && combo.Targets.Count > 0)
        {
            var targetsStr = new System.Text.StringBuilder();
            for (int i = 0; i < combo.Targets.Count; i++)
            {
                if (i > 0) targetsStr.Append(", ");
                targetsStr.Append(combo.Targets[i].ToString("F0"));
            }
            EditorGUILayout.LabelField("Targets", targetsStr.ToString(), EditorStyles.wordWrappedLabel);
        }

        EditorGUILayout.EndVertical();
    }

    private static void DrawBoolField(string label, bool value)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel(label);

        var prevColor = GUI.color;
        GUI.color = value ? Color.green : Color.gray;
        EditorGUILayout.LabelField(value ? "\u25a0 Yes" : "\u25a0 No",
            new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold },
            GUILayout.Width(50));
        GUI.color = prevColor;

        EditorGUILayout.EndHorizontal();
    }
}
