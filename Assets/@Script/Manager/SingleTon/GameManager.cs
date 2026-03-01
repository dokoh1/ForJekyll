using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using IGameManager;

public class GameManager : SingletonManager<GameManager>
{
    private IGameInterface _iGameInterface;
    private IMaterialChange _materialChange;
    public void Construct(IGameInterface iGameInterface, IMaterialChange materialChange)
    {
        _iGameInterface = iGameInterface;
        _materialChange = materialChange;
    }
    
    [TabGroup("Tab", "Manager", SdfIconType.GearFill, TextColor = "orange")]
    [TabGroup("Tab", "Manager")] public FadeManager fadeManager;
    [TabGroup("Tab", "Manager")] public LightManager lightManager;
    [TabGroup("Tab", "Manager")] public PlayerSettingManager playerSettingManager;
    [TabGroup("Tab", "Manager")] public NoteManager noteManager;
    [TabGroup("Tab", "Manager")] public GameManagerInsataller gameManagerInstaller;
    

    public Player Player 
    {
        get
        {
            if (_player == null) _player = FindAnyObjectByType<Player>();
            return _player;            
        }
        set { _player = value; }                      
    }
    public bool IsGameover
    {
        get { return _isGameover; }
        set
        {
            if (value) OnGameover?.Invoke();
            _isGameover = value;
        }
    }

    public bool IsPaused
    {
        get { return _isPaused; }
        set
        {
            if (value) OnPause?.Invoke();
            else OnResume?.Invoke();
            _isPaused = value;
        }
    }

    public bool IsTimeStop
    {
        get { return _isTimeStop; }
        set
        {
            if (value) OnTimeStop?.Invoke();
            else OffTimeStop?.Invoke();
            _isTimeStop = value;
        }
    }

    public event Action OnGameover;
    public event Action OnPause;
    public event Action OnResume;
    public event Action OnTimeStop;
    public event Action OffTimeStop;

    [SerializeField] private bool isSceneChange = false;
    [SerializeField] public SceneEnum sceneEnum;
    [SerializeField] private int chapter;

    private Player _player;
    private bool _isGameover;
    private bool _isPaused;
    private bool _isTimeStop;

    private void Start()
    {
        if (fadeManager == null) fadeManager = GetComponent<FadeManager>();
        if (lightManager == null) lightManager = GetComponent<LightManager>();
        if (playerSettingManager == null) playerSettingManager = GetComponent<PlayerSettingManager>();
        if (gameManagerInstaller == null)
        {
            gameManagerInstaller = GetComponent<GameManagerInsataller>();
        }
        gameManagerInstaller.Init(this);
#if UNITY_EDITOR
        StartCoroutine(SceneChange());
#endif
    }

    private IEnumerator SceneChange()
    {
        while (true)
        {
            if (isSceneChange)
            {
                ScenarioManager.Instance.SetAchieve(ScenarioAchieve.PlayerFlashLight, true);

                var target = sceneEnum;

                var belowEnums = Enum.GetValues(typeof(SceneEnum))
                    .Cast<SceneEnum>()
                    .Where(e => (int)e < (int)target)
                    .ToList();

                foreach (var e in belowEnums)
                {
                    if (Enum.TryParse<ScenarioAchieve>(e.ToString(), out var achieveEnum))
                    {
                        ScenarioManager.Instance.SetAchieve(achieveEnum, true);
                    }
                }
                fadeManager.MoveScene(sceneEnum);
                DataManager.Instance.Chapter = chapter;
                break;
            }
            yield return null;
        }
    }

    public void MovePlayerTransform(Transform transform)
    {
        Debug.Log("MovePlayerTransform");
        _iGameInterface.PlayerMove(transform, Player);
    }
    public void HighLightMaterialDelete(Renderer renderer, Material material)
    {
        _materialChange.HighLightMaterialDelete(renderer, material);
    }
    public void PlayerDiedCanvas(float time)
    {
        StartCoroutine(_iGameInterface.ShowDeathCanvas(time));
    }

    public void RetryBtn()
    {
        _iGameInterface.RetryBtn();
    }

    public void MenuBtn()
    {
        _iGameInterface.MenuBtn();
    }

    public void CursorVisible()
    {
        StartCoroutine(CursorLock());
    }

    private IEnumerator CursorLock()
    {
        yield return new WaitForSeconds(0.1f); // 1 프레임 기다림
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    #region 빌드 후 테스트용 최종 때 지우기
     private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && Input.GetKeyDown(KeyCode.Escape))
        {
            moveNextScene.SetActive(true);
        }
    }

    public GameObject moveNextScene;
    public void SceneChange(string sceneName)
    {
        Debug.Log("Scene changed to: " + sceneName);
        if (!int.TryParse(sceneName, out var sceneInt)) return;

        var sceneEnum = SceneEnum.Chapter1_1;
        switch (sceneInt)
        {
            case 0: DataManager.Instance.Chapter = 0; sceneEnum = SceneEnum.Tutorial; break;
            case 11: DataManager.Instance.Chapter = 1;sceneEnum = SceneEnum.Chapter1_1; break;
            case 12: DataManager.Instance.Chapter = 1;sceneEnum = SceneEnum.Chapter1_2; break;
            case 13: DataManager.Instance.Chapter = 1;sceneEnum = SceneEnum.Chapter1_3; break;
            case 21: DataManager.Instance.Chapter = 2;sceneEnum = SceneEnum.Chapter2_1; break;
            case 22: DataManager.Instance.Chapter = 2;sceneEnum = SceneEnum.Chapter2_2; break;
            case 23: DataManager.Instance.Chapter = 2;sceneEnum = SceneEnum.Chapter2_3; break;
            case 31: DataManager.Instance.Chapter = 3;sceneEnum = SceneEnum.Chapter3_1; break;
            case 32: DataManager.Instance.Chapter = 3;sceneEnum = SceneEnum.Chapter3_2; break;
            case 33: DataManager.Instance.Chapter = 3;sceneEnum = SceneEnum.Chapter3_1; break;
            case 41: DataManager.Instance.Chapter = 4;sceneEnum = SceneEnum.Chapter4_1; break;
            case 42: DataManager.Instance.Chapter = 4;sceneEnum = SceneEnum.Chapter4_2; break;
            case 43: DataManager.Instance.Chapter = 4;sceneEnum = SceneEnum.Chapter4_3; break;
        }
        
        ScenarioManager.Instance.SetAchieve(ScenarioAchieve.PlayerFlashLight, true);
        
        var belowEnums = Enum.GetValues(typeof(SceneEnum))
            .Cast<SceneEnum>()
            .Where(e => (int)e < (int)sceneEnum)
            .ToList();

        foreach (var e in belowEnums)
        {
            if (Enum.TryParse<ScenarioAchieve>(e.ToString(), out var achieveEnum)) 
            { ScenarioManager.Instance.SetAchieve(achieveEnum, true); }
        }
        
        fadeManager.MoveScene(sceneEnum);
        
        moveNextScene.SetActive(false);
    }

    #endregion
   
}
