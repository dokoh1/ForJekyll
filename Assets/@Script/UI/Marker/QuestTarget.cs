using UnityEngine;

namespace Marker
{
    public class QuestTarget : MonoBehaviour
    {
        [SerializeField] private Transform anchor;
        [SerializeField] private float activationDistance = 12f;
        [SerializeField] private float interactionDistance = 1.5f;
        [SerializeField] public float yOffset = 40f;

        public Transform Anchor => anchor ? anchor : transform;
        public float ActivationDistance => activationDistance;
        public float InteractionDistance => interactionDistance;
    }
}
