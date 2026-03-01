using System.Collections;
using IGameManager;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace IGameManager
{
    public interface IGamePlayer
    {
        void PlayerMove(Transform transform, Player player);
        void PlayerDead();
    }

    public interface IMaterialChange
    {
        void HighLightMaterialDelete(Renderer renderer, Material material);
    }

    public interface IPlayerDeathCanvas
    {
        IEnumerator ShowDeathCanvas(float time);
        void RetryBtn();
        void MenuBtn();
    }

    public interface IGameInterface : IGamePlayer, IPlayerDeathCanvas {}
    
    public class PlayerMovement : IGameInterface
    {
        [Header("PlayerDied")]
        private GameObject BlackImage;
        private GameObject DiedVideo;
        private Button ReBtn;
        private Button MainBtn;

        public void ObjectInitialize(GameObject blackImage, GameObject diedVideo, Button reBtn, Button mainBtn)
        {
            BlackImage = blackImage;
            DiedVideo = diedVideo;
            ReBtn = reBtn;
            MainBtn = mainBtn;
        }
        public void PlayerMove(Transform transform, Player player)
        {
            CharacterController con = player.Controller;
            con.enabled = false;
            player.transform.position = transform.position;
            con.enabled = true;
        }

        public void PlayerRotation(Transform transform, Player player)
        {
            CharacterController con = player.Controller;
            con.enabled = false;
            player.transform.rotation = transform.rotation;
            con.enabled = true;
        }

        public void PlayerDead()
        {
            
        }

        public IEnumerator ShowDeathCanvas(float time)
        {
            yield return new WaitForSeconds(time);
            if (SoundManager.Instance.SFX_Source.isPlaying)
            {
                SoundManager.Instance.SFX_Source.Stop();
            }
            //BlackImage.SetActive(true);
            //yield return new WaitForSeconds(3f);
            //DiedVideo.SetActive(true);
            yield return new WaitForSeconds(1f);
            ReBtn.gameObject.SetActive(true);
            MainBtn.gameObject.SetActive(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        public void RetryBtn()
        {
            DataManager.Instance.sceneDataManager.AutoDataLoad();
            GameManager.Instance.fadeManager.fadeComplete += OutFade;
            ReBtn.interactable = false;
            MainBtn.interactable = false;
        }
        private void OutFade()
        {
            ScenarioManager.Instance.SetAchieve(PlayerAchieve.PlayerStop, false);
            DiedVideo.SetActive(false);
            BlackImage.SetActive(false);
            ReBtn.gameObject.SetActive(false);
            MainBtn.gameObject.SetActive(false);
            ReBtn.interactable = true;
            MainBtn.interactable = true;
            UI_PauseMenuPopup.Instance.SwitchActiveESC(true);
        }
        public void MenuBtn()
        {
            GameManager.Instance.fadeManager.MoveScene(SceneEnum.MainMenu);
            GameManager.Instance.fadeManager.fadeComplete += OutFade;
            ReBtn.interactable = false;
            MainBtn.interactable = false;
        }
    }

    public class MaterialsChange : IMaterialChange
    {
        public void HighLightMaterialDelete(Renderer renderer, Material changeMaterial)
        {
            Material[] materials = renderer.materials;

            for (int i = 0; i < materials.Length; i++)
            {
                string materialName = materials[i].name.Replace(" (Instance)", "");

                if (materialName == "Highlight")
                {
                    materials[i] = changeMaterial;
                }
            }
            renderer.materials = materials;
        }
    }
}
public class GameManagerInsataller : MonoBehaviour
{    
    [SerializeField] private GameObject BlackImage;
    [SerializeField] private GameObject DiedVideo;
    [SerializeField] private Button ReBtn;
    [SerializeField] private Button MainBtn;
    
    private GameManager gameManager;
    public void Init(GameManager gameManager)
    {
        this.gameManager = gameManager;
        
        PlayerMovement playerMovement = new PlayerMovement();
        MaterialsChange materialsChange = new MaterialsChange();
        
        playerMovement.ObjectInitialize(BlackImage, DiedVideo, ReBtn, MainBtn);
        
        gameManager.Construct(playerMovement, materialsChange);
    }
}



