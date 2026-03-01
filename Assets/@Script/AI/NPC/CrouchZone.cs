using NPC;
using UnityEngine;

public class CrouchZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7 || other.gameObject.layer == 13)
        {
            if (other.gameObject.TryGetComponent<Npc>(out var npc)) npc.SetCrouch(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7 || other.gameObject.layer == 13)
        {
            if (other.gameObject.TryGetComponent<Npc>(out var npc)) npc.SetCrouch(false);
        }
    }
}