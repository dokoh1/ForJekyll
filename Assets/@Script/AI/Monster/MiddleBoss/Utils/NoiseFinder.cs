using UnityEngine;

/// <summary>
/// OverlapSphere + INoise 탐색 유틸리티.
/// MBDetectNoiseService, MidBossRage 등에서 공통 사용.
/// </summary>
public static class NoiseFinder
{
    private static readonly Collider[] Buffer = new Collider[64];

    public static bool TryFindLoudest(
        Vector3 origin,
        float range,
        LayerMask noiseMask,
        out Vector3 position,
        out float loudestAmount)
    {
        position = Vector3.zero;
        loudestAmount = 0f;

        int count = Physics.OverlapSphereNonAlloc(origin, range, Buffer, noiseMask);
        bool found = false;

        for (int i = 0; i < count; i++)
        {
            if (Buffer[i] == null)
                continue;

            if (Buffer[i].TryGetComponent<INoise>(out var noise))
            {
                float n = noise.NoiseCheckAmount;
                if (n > loudestAmount)
                {
                    loudestAmount = n;
                    position = Buffer[i].bounds.center;
                    found = true;
                }
            }
        }

        return found;
    }
}
