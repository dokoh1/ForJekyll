using DG.Tweening;
using UnityEngine;

namespace Vent
{
    public class VentLaver
    {
        public LaverInteract laverInteract;
        public int number;

        public VentLaver(LaverInteract laverInteract, int number)
        {
            this.laverInteract = laverInteract;
            this.number = number;
        }
    }

    public class LaverInteract : InteractableBase
    {
        [field: Header("InteractType")]
        [field: SerializeField] public override InteractTypeEnum InteractType { get; set; } = InteractTypeEnum.Tap;

        [field: Header("Interactable")]
        [field: SerializeField] public override bool IsInteractable { get; set; } = false;
        [field: SerializeField] public override float InteractHoldTime { get; set; }

        [SerializeField] private DOTweenAnimation laverUp;
        [SerializeField] private DOTweenAnimation laverDown;

        [SerializeField] private LaverPuzzle laverPuzzle;
        [SerializeField] private int number;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Light light;

        public override void Interact()
        {
            laverUp.DOKill();
            laverUp.CreateTween(true);
            audioSource.Play();
            
            laverPuzzle.SelectNumber(number);
            IsInteractable = false;
        }

        private void Awake()
        {
            var ventLaver = new VentLaver(this, number);
            laverPuzzle.AddVentLaver(ventLaver);
            
            if (audioSource == null) audioSource = GetComponent<AudioSource>();
        }

        public void LaverUp()
        {
            light.enabled = true;
        }
    
        public void LaverReset()
        {
            laverDown.DOKill();
            laverDown.CreateTween(true);
        }
    } 
}

