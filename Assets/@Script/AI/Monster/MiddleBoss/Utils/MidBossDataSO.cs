using System;
using UnityEngine;

[CreateAssetMenu(fileName = "MidBossDataSO", menuName = "SO/Unit/Monster/MidBoss")]
public class MidBossDataSO : ScriptableObject
{
    [field: SerializeField] public MidBossData Data { get; private set; }

    [Serializable]
    public class MidBossData
    {
        [Header("Door / Wall")]
        public float requiredDoorDistance = 12f;  // 문 파괴 최소 거리
        public int   wallWarn1Count      = 1;     // 경고 1
        public int   wallWarn2Count      = 2;     // 경고 2
        public int   wallDeathCount      = 3;     // 붕괴 + 처형

        [Header("Stun (Phase 1 / 2)")]
        public float baseStunDuration    = 2f;    // 1페이즈 스턴 시간
        public float enragedStunDuration = 1.2f;  // 2페이즈 스턴 시간

        [Header("Enrage (Phase 2)")]
        public int   comboMaxCharges = 3;
        public float moveSpeedMultiplierOnEnrage = 1.2f; // 필요시 사용
        public float noiseGainMultiplierOnEnrage = 2f;   // Noise 게이지 배율

        [Header("Gauge Thresholds")]
        public float detectMin = 30f; // Detect 상태 진입 기준
        public float rageMin   = 50f; // Rage/Charge 진입 기준

        [Header("Detect")]
        public float detectRangeMax = 1000f;
        
        [Header("Charge")]
        public float chargeSpeed   = 15f;
        public float maxChargeTime = 3f;
        public float stopDistance  = 1.5f;
        public float chargeOvershoot = 10f;

        [Header("Fallback")]
        public float randomNavRadius = 5f;
        public float rageFallbackDistance = 3f;
    }
}