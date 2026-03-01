using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FitBoxColliderToChildren : MonoBehaviour
{
    [ContextMenu("Fit BoxCollider To Children")]
    private void Fit()
    {
        var box = GetComponent<BoxCollider>();
        var renderers = GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("자식에 Renderer가 없습니다.");
            return;
        }
        
        Bounds bounds = renderers[0].bounds;
        
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);
        
        Vector3 localCenter = transform.InverseTransformPoint(bounds.center);

        Vector3 lossy = transform.lossyScale;
        Vector3 localSize = new Vector3(
            bounds.size.x / Mathf.Max(lossy.x, 0.0001f),
            bounds.size.y / Mathf.Max(lossy.y, 0.0001f),
            bounds.size.z / Mathf.Max(lossy.z, 0.0001f)
        );
        
        box.center = localCenter;
        box.size = localSize;
        
        Debug.Log("[FitBoxColliderToChildren] 콜라이더 정렬 완료", this);
    }
}