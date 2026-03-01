using DG.Tweening;
using UnityEngine;

public class CloseCageObj : MonoBehaviour
    {
        [SerializeField] private ColliderHandler colliderHandler; 
        [SerializeField] private DOTweenAnimation closeAni;
        [SerializeField] private DOTweenAnimation openAni; 
        
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip openSound;
        [SerializeField] private AudioClip closeSound;

        private void Start()
        {
            if (colliderHandler != null)
            {
                colliderHandler.OnTriggerEntered += OnCloseAnimation;
            }
        }

        private void OnCloseAnimation()
        {
            closeAni.DOKill();
            closeAni.CreateTween(true);
            
            if (audioSource == null || closeSound == null) return;
            audioSource.clip = closeSound;
            audioSource.Play();
        }

        public void OpenAnimation()
        {
            openAni.DOKill();
            openAni.CreateTween(true);
            
            if (audioSource == null || openSound == null) return;
            audioSource.clip = openSound;
            audioSource.Play();
        }
    }