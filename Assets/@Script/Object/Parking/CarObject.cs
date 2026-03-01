using System.Collections;
using UnityEngine;

namespace Item
{
    public class CarObject : MonoBehaviour, INoise
    {
        [field: SerializeField] public bool CanRepeat { get; set; }// �ݺ� ��밡��

        // INoise
        [field: SerializeField] public float CurNoiseAmount { get; set; }
        [field: SerializeField] public float NoiseCheckAmount { get; set; }
        public float SumNoiseAmount { get; set; }
        public float DecreaseSpeed { get; set; }
        public float MaxNoiseAmount { get; set; }

        // player ui    
        public string ObjectName { get; set; }
        public string InteractKey { get; set; }
        public string InteractType { get; set; }

        //NoiseItemData
        [field: Header("NoiseData")]
        public float NoiseTimer { get; set; }
        [field: SerializeField] public bool Sound3D { get; set; }
        //public List<ObjectSoundData> SoundList;
        [field: SerializeField] private ObjectSoundData SoundData { get; set; }
        public AudioSource AS;
        Collider col;

        [SerializeField] private Light[] carLight;
        [SerializeField] private Material[] lightMaterial;
        [SerializeField] private MeshRenderer carMeshRenderer;

        private void Start()
        {
            //ObjectName = NoiseItemData.Data.ObjectName;
            //InteractKey = NoiseItemData.Data.InteractKey;
            //InteractType = NoiseItemData.Data.InteractType;
            //NoiseTimer = NoiseItemData.Data.NoiseTimer;

            //MaxNoiseAmount = NoiseItemData.Data.MaxNoiseAmount;
            //DecreaseSpeed = NoiseItemData.Data.DecreaseSpeed;

            if (AS == null) AS = gameObject.GetComponent<AudioSource>();
            //if (SoundData != null) AS.clip = SoundData.noises[0];
            if (Sound3D) Sound3DSetting();
            col = GetComponent<Collider>();
        }
        bool oneTime = false;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !oneTime)
            {
                col.isTrigger = false;
                oneTime = true;
                float randomChance = Random.value;
                if (randomChance <= 0.5f)
                    PlayNoise(NoiseTimer);
                StartCoroutine(CarLight());
                StartCoroutine(CarLightMaterial());
            }
        }
        IEnumerator CarLightMaterial()
        {
            float time = 0;
            Material[] materials = carMeshRenderer.materials;
            while(time < AS.clip.length)
            {
                materials[0] = lightMaterial[0];
                carMeshRenderer.material = materials[0];

                yield return new WaitForSeconds(1f);
                time += 1f;
                materials[0] = lightMaterial[1];
                carMeshRenderer.material = materials[0];
                yield return new WaitForSeconds(1f);
                time += 1f;
            }
            materials[0] = lightMaterial[1];
            carMeshRenderer.material = materials[0];
        }
        IEnumerator CarLight()
        {
            float time = 0;
            while (time < AS.clip.length)
            {
                foreach(var light in carLight)
                {
                    light.gameObject.SetActive(!light.gameObject.activeSelf);
                }
                yield return new WaitForSeconds(1f);
                time += 1f;
            }
            foreach (var light in carLight)
            {
                light.gameObject.SetActive(false);
            }
        }
        public void PlayNoise(float duration)
        {
            AS.Play();

            if (AS != null)
            {
                if (NoiseTimer > 0)
                {
                    AS.loop = true;
                    CurNoiseAmount = SoundData.noiseAmount;
                    AS.Play();
                    StartCoroutine(StopNoiseAfterDuration(duration));
                }
                else
                {
                    AS.loop = false;
                    CurNoiseAmount = SoundData.noiseAmount;
                    AS.Play();
                    StartCoroutine(StopNoiseAfterDuration(AS.clip.length));
                }
            }
        }

        private IEnumerator StopNoiseAfterDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            CurNoiseAmount = 0f;
            AS.Stop();
        }

        private void Sound3DSetting()
        {
            if (AS != null)
            {
                AS.spatialBlend = 1f;
                AS.dopplerLevel = 0f;
                AS.rolloffMode = AudioRolloffMode.Linear;
                AS.maxDistance = 30f;
            }
        }

        public void TriggerEffect()
        {
            StartCoroutine(CarLight());
            StartCoroutine(CarLightMaterial());
            PlayNoise(NoiseTimer);
        }
    }
}