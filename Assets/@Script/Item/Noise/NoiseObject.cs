using System.Collections;
using UnityEngine;

namespace Item
{
    public class NoiseObject : InteractableBase, INoise
    {
        public enum NoiseWorkTypeEnum
        {
            Tap,
            Hold,
            TapAndHold
        }

        [field: Header("InteractType")]
        [field: SerializeField] public override InteractTypeEnum InteractType { get; set; }
        [field: SerializeField] public NoiseWorkTypeEnum NoiseWorkType { get; set; }

        // IInteractable
        [field: Header("Interactable")]
        [field: SerializeField] public override bool IsInteractable { get; set; }
        [field: SerializeField] public override float InteractHoldTime { get; set; }

        [field: SerializeField] public bool CanRepeat { get; set; }
        [field: SerializeField] public bool CanTapRepeat { get; set; }
        [field: SerializeField] public bool CanHoldRepeat { get; set; }

        // INoise
        [field: SerializeField] public float CurNoiseAmount { get; set; }
        [field: SerializeField] public float NoiseCheckAmount { get; set; }
        public float SumNoiseAmount { get; set; }
        public float DecreaseSpeed { get; set; }
        public float MaxNoiseAmount { get; set; }

        //NoiseItemData
        [field: Header("NoiseData")]
        [field: SerializeField] public float TapNoiseTimer { get; set; }
        [field: SerializeField] public float HoldNoiseTimer { get; set; }
        //[field: SerializeField] public bool IsPlayingNoise { get; set; }
        private bool _isPlayingNoiseTabAndHold = false;

        //public List<ObjectSoundData> SoundList;
        [field: SerializeField] public ObjectSoundData TapSoundData { get; set; }
        [field: SerializeField] public ObjectSoundData HoldSoundData { get; set; }

        public AudioSource tapAS;
        public AudioSource holdAS;

        [field: Header("Sound3D")]
        [field: SerializeField] private bool Sound3D { get; set; }
        [field: SerializeField] private float maxDistance3D { get; set; } = 30f;

        protected override void Start()
        {
            base.Start();

            if (InteractType == InteractTypeEnum.Tap)
            {
                if (TapSoundData.noises.Length == 0)
                {
                    Debug.LogError($"NoiseObject - TapSoundData is null - {gameObject.name}");
                }
            }
            else if (InteractType == InteractTypeEnum.Hold)
            {
                if (HoldSoundData.noises.Length == 0)
                {
                    Debug.LogError($"NoiseObject - HoldSoundData is null - {gameObject.name}");
                }
            }
            else if ( InteractType == InteractTypeEnum.TapAndHold && NoiseWorkType == NoiseWorkTypeEnum.TapAndHold)
            {
                if (TapSoundData.noises.Length == 0 || HoldSoundData.noises.Length == 0)
                {
                    Debug.LogError($"NoiseObject - Tap or Hold SoundData SoundData is null - {gameObject.name}");
                }
            }

            if (holdAS == null) holdAS = gameObject.GetComponent<AudioSource>();
            if (holdAS == null || tapAS == null)
            {
                Debug.LogError($"NoiseObject - AudioSource is null - {gameObject.name}");
            }

            //if (SoundData != null) AS.clip = SoundData.noises[0];
            if (Sound3D) Sound3DSetting();
        }

        private void FixedUpdate()
        {
            if (holdAS.isPlaying && GameManager.Instance.IsGameover)
            {
                if (CanRepeat) IsInteractable = true;
                CurNoiseAmount = 0f;
                holdAS.Stop();
            }
        }

        public override void Interact()
        {
            //Debug.Log("NoiseObject - Interact() called");

            if (NoiseWorkType == NoiseWorkTypeEnum.Tap)
            {
                IsInteractable = false;
                PlayNoise(TapNoiseTimer, TapSoundData, tapAS);
                //Debug.Log($"NoiseObject - Tap 소음 - t");
                return;
            }
            else if (NoiseWorkType == NoiseWorkTypeEnum.TapAndHold)
            {
                if (InteractType == InteractTypeEnum.TapAndHold)
                {
                    if (!CanTap || _isPlayingNoiseTabAndHold) return;

                    CanTap = false;
                    _isPlayingNoiseTabAndHold = true;
                }

                IsInteractable = false;
                PlayNoise(TapNoiseTimer, TapSoundData, tapAS);
                //Debug.Log($"NoiseObject - Tap 소음 - tah");
                return;
            }

            //Debug.Log($"NoiseObject - Tap 상호 작용");
            if (!CanRepeat) IsInteractable = false;
        }

        public override void Hold()
        {
            //Debug.Log("NoiseObject - Hold() called");

            if (NoiseWorkType == NoiseWorkTypeEnum.Hold)
            {
                if (InteractType == InteractTypeEnum.TapAndHold && !CanHold) return;

                IsInteractable = false;
                PlayNoise(HoldNoiseTimer, HoldSoundData, holdAS);
                CanHold = false;
                //Debug.Log($"NoiseObject - Hold 소음 - h");
                return;
            }
            else if (NoiseWorkType == NoiseWorkTypeEnum.TapAndHold)
            {
                if (InteractType == InteractTypeEnum.TapAndHold)
                {
                    if (!CanHold || _isPlayingNoiseTabAndHold) return;

                    CanHold = false;
                    _isPlayingNoiseTabAndHold = true;
                }

                IsInteractable = false;
                PlayNoise(HoldNoiseTimer, HoldSoundData, holdAS);
                //Debug.Log($"NoiseObject - Hold 소음 - tah");
                return;
            }

            //Debug.Log($"NoiseObject - Hold 상호 작용");            
            if (!CanRepeat) IsInteractable = false;

        }

        public void PlayNoise(float duration, ObjectSoundData soundData, AudioSource audioSource , int index = 0)
        {
            audioSource.clip = soundData.noises[index];
            IsInteractable = false;

            if (audioSource != null)
            {
                if (duration > 0)
                {
                    audioSource.loop = true;
                    CurNoiseAmount = soundData.noiseAmount;
                    audioSource.Play();
                    StartCoroutine(StopNoiseAfterDuration(duration, audioSource));
                }
                else
                {
                    audioSource.loop = false;
                    CurNoiseAmount = soundData.noiseAmount;
                    audioSource.Play();
                    StartCoroutine(StopNoiseAfterDuration(audioSource.clip.length, audioSource));
                }
            }
        }

        public void PlayNoise(float duration, ObjectSoundData soundData)
        {
            holdAS.clip = soundData.noises[0];
            IsInteractable = false;

            if (holdAS != null)
            {
                if (duration > 0)
                {
                    holdAS.loop = true;
                    CurNoiseAmount = soundData.noiseAmount;
                    holdAS.Play();
                    StartCoroutine(StopNoiseAfterDuration(duration));
                }
                else
                {
                    holdAS.loop = false;
                    CurNoiseAmount = soundData.noiseAmount;
                    holdAS.Play();
                    StartCoroutine(StopNoiseAfterDuration(holdAS.clip.length));
                }
            }
        }

        public void PlayNoise(float duration)
        {
            holdAS.clip = TapSoundData.noises[0];
            IsInteractable = false;

            if (holdAS != null)
            {
                if (TapNoiseTimer > 0)
                {
                    holdAS.loop = true;
                    CurNoiseAmount = TapSoundData.noiseAmount;
                    holdAS.Play();
                    StartCoroutine(StopNoiseAfterDuration(duration));
                }
                else 
                {
                    holdAS.loop = false;
                    CurNoiseAmount = TapSoundData.noiseAmount;
                    holdAS.Play();
                    StartCoroutine(StopNoiseAfterDuration(holdAS.clip.length));
                }                
            }
        }

        public void PlayNoise(float duration, int index)
        {
            holdAS.clip = TapSoundData.noises[index];
            IsInteractable = false;

            if (holdAS != null)
            {
                if (TapNoiseTimer > 0)
                {
                    holdAS.loop = true;
                    CurNoiseAmount = TapSoundData.noiseAmount;
                    holdAS.Play();
                    StartCoroutine(StopNoiseAfterDuration(duration));
                }
                else
                {
                    holdAS.loop = false;
                    CurNoiseAmount = TapSoundData.noiseAmount;
                    holdAS.Play();
                    StartCoroutine(StopNoiseAfterDuration(holdAS.clip.length));
                }
            }
        }

        private IEnumerator StopNoiseAfterDuration(float duration, AudioSource audioSource)
        {
            yield return new WaitForSeconds(duration);
            if (CanRepeat) IsInteractable = true;
            if (NoiseWorkType == NoiseWorkTypeEnum.TapAndHold || InteractType == InteractTypeEnum.TapAndHold)
            {
                if (CanTapRepeat) CanTap = true;
                if (CanHoldRepeat) CanHold = true;

                //Debug.Log($"CanTap : {CanTap}, CanHold : {CanHold}");

                _isPlayingNoiseTabAndHold = false;
            }
            CurNoiseAmount = 0f;
            audioSource.Stop();
        }


        private IEnumerator StopNoiseAfterDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (CanRepeat) IsInteractable = true;
            if (NoiseWorkType == NoiseWorkTypeEnum.TapAndHold || InteractType == InteractTypeEnum.TapAndHold)
            {
                if (CanTapRepeat) CanTap = true;
                if (CanHoldRepeat) CanHold = true;

                //Debug.Log($"CanTap : {CanTap}, CanHold : {CanHold}");
                
                _isPlayingNoiseTabAndHold = false;
            }
            CurNoiseAmount = 0f;
            holdAS.Stop();
        }

        private void Sound3DSetting()
        {
            if (holdAS != null)
            {
                holdAS.spatialBlend = 1f;
                holdAS.dopplerLevel = 0f;
                holdAS.rolloffMode = AudioRolloffMode.Linear;
                holdAS.maxDistance = maxDistance3D;
            }

            if (tapAS != null)
            {
                tapAS.spatialBlend = 1f;
                tapAS.dopplerLevel = 0f;
                tapAS.rolloffMode = AudioRolloffMode.Linear;
                tapAS.maxDistance = maxDistance3D;
            }
        }

        //public void PauseAudio()
        //{
        //    if (tapAS.isPlaying) tapAS.Pause();
        //    if (holdAS.isPlaying) holdAS.Pause();
        //}

        //public void ResumeAudio()
        //{
        //    if (!tapAS.isPlaying) tapAS.UnPause();
        //    if (!holdAS.isPlaying) holdAS.UnPause();
        //}

        //private void OnEnable()
        //{
        //    GameManager.Instance.OnTimeStop += PauseAudio;
        //    GameManager.Instance.OffTimeStop += ResumeAudio;
        //}

        //private void OnDisable()
        //{
        //    GameManager.Instance.OnTimeStop -= PauseAudio;
        //    GameManager.Instance.OffTimeStop -= ResumeAudio;
        //}
    }
}