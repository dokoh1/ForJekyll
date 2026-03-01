using UnityEngine;

public class FloorPosition : MonoBehaviour
{
    public Transform floorTopPosition;
    public Transform floorBottomPosition;

    private void Start()
    {
        GameManager.Instance.lightManager.floorTopTransforms.Add(floorTopPosition);
        GameManager.Instance.lightManager.floorBottomTransforms.Add(floorBottomPosition);

        GameManager.Instance.lightManager.SortFloorTransform();
    }
}
