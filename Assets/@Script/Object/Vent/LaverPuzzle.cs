using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Vent
{
    public class LaverPuzzle : MonoBehaviour
    {
        [Header("Puzzle")]
        [SerializeField] private int count = 4;
        [SerializeField] private List<int> puzzleNumbers = new();
        [SerializeField] private List<int> curNumbers = new();

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip successClipSound;
        [SerializeField] private AudioClip failClipSound;

        [Header("Renderer")] 
        [SerializeField] private GameObject arlmPivot;
        [SerializeField] private Renderer arlmObject;
        [SerializeField] private Material noneMaterial;
        [SerializeField] private Material redMaterial;
        [SerializeField] private Material greenMaterial;

        [Header("Light")]
        [SerializeField] private Light redLight;
        [SerializeField] private Light greenLight;
        
        [Header("Animation")]
        [SerializeField] private DOTweenAnimation openIronDoorAnimation;

        [ShowInInspector, ReadOnly]
        private List<VentWarningLight> warnings = new();
        
        [ShowInInspector, ReadOnly]
        private List<VentLaver> lavers = new();
        
        private WaitForSeconds delay = new WaitForSeconds(2f);

        public void AddVentWarningLight(VentWarningLight warning) { warnings.Add(warning); }
        public void AddVentLaver(VentLaver laver) { lavers.Add(laver); }

        private void Start()
        {
            StartCoroutine(GameStart());
        }

        public IEnumerator GameStart()
        {
            puzzleNumbers.Clear();
            curNumbers.Clear();

            foreach (var r in lavers)
            {
                r.laverInteract.LaverReset();
            }
            
            yield return StartCoroutine(ChooseNumber(count));
            yield return PuzzleStart();
        }

        private IEnumerator ChooseNumber(int ii)
        {
            for (var i = 0; i < ii; i++)
            {
                while (true)
                {
                    var num = Random.Range(1, 9); // 1~8
                    
                    if (puzzleNumbers.Contains(num)) continue;
                
                    puzzleNumbers.Add(num);
                    break;
                }
            }
            yield return null;
        }

        private IEnumerator PuzzleStart()
        {
            yield return delay;
            
            arlmPivot.SetActive(false);
            redLight.enabled = false;
            greenLight.enabled = false;
            
            foreach (var w in warnings)
            {
                w.warningLight.OffLight();
            }
            
            foreach (var i in puzzleNumbers)
            {
                foreach (var w in warnings)
                {
                    if (w.number == i)
                    {
                        yield return StartCoroutine(w.warningLight.OnLight());
                        break;
                    }
                }
            }

            foreach (var l in lavers)
            {
                l.laverInteract.IsInteractable = true;
            }
            
            yield return null;
        }

        public void SelectNumber(int num)
        {
            curNumbers.Add(num);

            if (curNumbers.Count == count)
            {
                foreach (var l in lavers)
                {
                    l.laverInteract.IsInteractable = false;
                }
                
                CheckNumbers();
            }
        }

        private void CheckNumbers()
        {
            for (var i = 0; i < puzzleNumbers.Count; i++)
            {
                if (puzzleNumbers[i] != curNumbers[i])
                {
                    var m = arlmObject.materials;
                    m[0] = redMaterial;
                    
                    StopAllCoroutines();
                    FailPuzzle();
                    return;
                }
            }
            
            var t = arlmObject.materials;
            t[0] = greenMaterial;
            
            StopAllCoroutines();
            SuccessPuzzle();
        }

        public void SuccessPuzzle() { StartCoroutine(Puzzle(true)); }
        public void FailPuzzle() { StartCoroutine(Puzzle(false)); }
        private IEnumerator Puzzle(bool check)
        {
            yield return delay;
            arlmPivot.SetActive(true);
            
            if (check)
            {
                greenLight.enabled = true;
                yield return delay;
                
                openIronDoorAnimation.DOKill();
                openIronDoorAnimation.CreateTween(true);
            }
            else
            {
                redLight.enabled = true;
                yield return delay;
                redLight.enabled = false;
                
                var m = arlmObject.materials;
                m[0] = noneMaterial;
                StartCoroutine(GameStart());
            }
            yield return null;
        }
    }
}
