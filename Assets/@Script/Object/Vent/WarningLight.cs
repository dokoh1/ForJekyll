using System;
using System.Collections;
using UnityEngine;


namespace Vent
{
    public class VentWarningLight
    {
        public WarningLight warningLight;
        public int number;

        public VentWarningLight(WarningLight warningLight, int number)
        {
            this.warningLight = warningLight;
            this.number = number;
        }
    }
    
    public class WarningLight : MonoBehaviour
    {
        [SerializeField] LaverPuzzle laverPuzzle;
        [SerializeField] private int number;
        
        [SerializeField] private Renderer renderer;
        [SerializeField] private Material noneMaterial;
        [SerializeField] private Material lightMaterial;
        [SerializeField] private Light light;

        private WaitForSeconds wait = new WaitForSeconds(1f); 
        private void Awake()
        {
            VentWarningLight warningLight = new(this, number);
            laverPuzzle.AddVentWarningLight(warningLight);
        }

        public void GameStart()
        {
            var m = renderer.materials;
            m[0] = noneMaterial;
        }
        
        public IEnumerator OnLight()
        {
            var m = renderer.materials;
            
            light.enabled = true;
            m[0] = lightMaterial;

            yield return wait;
            
            light.enabled = false;
            m[0] = noneMaterial;
        }

        public void OffLight()
        {
            light.enabled = false;
        }
    }
}

