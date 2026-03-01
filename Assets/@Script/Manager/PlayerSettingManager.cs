using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerSettingManager : MonoBehaviour
{
    private ScenarioManager _scenarioManager;
    private GameManager _gameManager;
    private Dictionary<PlayerAchieve, Action> playerSettings;
    private void Awake()
    {
        playerSettings = new Dictionary<PlayerAchieve, Action>()
        {
            { PlayerAchieve.PlayerMove , PlayerMove},
            { PlayerAchieve.PlayerStop , PlayerStop},
            
            { PlayerAchieve.Dialogue, DialogueOrSavingOrMenu},
            { PlayerAchieve.Saving, DialogueOrSavingOrMenu},
            { PlayerAchieve.EscMenu, Menu},
            
            { PlayerAchieve.ObjectInteract, InteractOrCutScene},
            { PlayerAchieve.CutScenePlaying, InteractOrCutScene},
            
            { PlayerAchieve.OnlyInteractLocked, InteractLock},
            { PlayerAchieve.LookAndInteract, LookAndInteract},
            { PlayerAchieve.OnlyLook, OnlyLook},
            { PlayerAchieve.FlashLockOnlyLook, FlashLockOnlyLook}
        };
    }

    private void Start()
    {
        _scenarioManager = ScenarioManager.Instance;
        _gameManager = GameManager.Instance;
    }

    public void SetState(PlayerAchieve playerAchieve, bool state)
    {
        switch (playerAchieve)
        {
            case PlayerAchieve.PlayerMove:
            case PlayerAchieve.PlayerStop:
                break;
            case PlayerAchieve.Dialogue:
                
                UIManager.Instance.questUI.ToggleQuestUI(!state);
                UIManager.Instance.objective.SetHidden(HideReason.Dialogue, state);
                if (ScenarioManager.Instance.GetAchieve(PlayerAchieve.LookAndInteract)
                    || _scenarioManager.GetAchieve(PlayerAchieve.ObjectInteract)
                    || _scenarioManager.GetAchieve(PlayerAchieve.OnlyInteractLocked)
                    || _scenarioManager.GetAchieve(PlayerAchieve.Saving)
                    || _scenarioManager.GetAchieve(PlayerAchieve.EscMenu)
                    || _scenarioManager.GetAchieve(PlayerAchieve.CutScenePlaying)
                    || _scenarioManager.GetAchieve(PlayerAchieve.OnlyLook))
                {
                    _scenarioManager.SetAchieve(playerAchieve,state);
                    return;
                }
                break;
            case PlayerAchieve.Saving:
            case PlayerAchieve.ObjectInteract:
                break;
            case PlayerAchieve.CutScenePlaying:
                
                GameManager.Instance.Player.PlayerCrosshairUI.SetActive(!state);
                UIManager.Instance.questUI.ToggleQuestUI(!state);
                UIManager.Instance.objective.SetHidden(HideReason.Cutscene, state);
                
                if (_scenarioManager.GetAchieve(PlayerAchieve.LookAndInteract))
                {
                    playerSettings[PlayerAchieve.LookAndInteract]?.Invoke();
                    _scenarioManager.SetAchieve(playerAchieve,state);
                    return;
                }
                break;
            case PlayerAchieve.OnlyInteractLocked:
                if (_scenarioManager.GetAchieve(PlayerAchieve.LookAndInteract)
                    || _scenarioManager.GetAchieve(PlayerAchieve.ObjectInteract)
                    || _scenarioManager.GetAchieve(PlayerAchieve.Saving)
                    || _scenarioManager.GetAchieve(PlayerAchieve.EscMenu)
                    || _scenarioManager.GetAchieve(PlayerAchieve.CutScenePlaying)
                    || _scenarioManager.GetAchieve(PlayerAchieve.Dialogue))
                {
                    _scenarioManager.SetAchieve(playerAchieve,state);
                    return;
                }
                break;
            case PlayerAchieve.LookAndInteract:
                if (_scenarioManager.GetAchieve(PlayerAchieve.CutScenePlaying))
                {
                    _scenarioManager.SetAchieve(playerAchieve,state);
                    return;
                }
                break;
            case PlayerAchieve.EscMenu:
                if (_scenarioManager.GetAchieve(PlayerAchieve.Saving))
                {
                    return;
                }
                
                if (_scenarioManager.GetAchieve(PlayerAchieve.EscMenu))
                {
                    _scenarioManager.SetAchieve(playerAchieve,state);
                    if (_scenarioManager.GetAchieve(PlayerAchieve.Dialogue))
                    {
                        playerSettings[PlayerAchieve.Dialogue]?.Invoke(); return;
                    }
                    if (_scenarioManager.GetAchieve(PlayerAchieve.ObjectInteract)
                             || _scenarioManager.GetAchieve(PlayerAchieve.CutScenePlaying))
                    {
                        playerSettings[PlayerAchieve.ObjectInteract]?.Invoke(); return;
                    }
                    if (_scenarioManager.GetAchieve(PlayerAchieve.OnlyInteractLocked))
                    {
                        playerSettings[PlayerAchieve.OnlyInteractLocked]?.Invoke(); return;
                    }
                    if (_scenarioManager.GetAchieve(PlayerAchieve.LookAndInteract))
                    {
                        playerSettings[PlayerAchieve.LookAndInteract]?.Invoke(); return;
                    }
                }
                break;
            case PlayerAchieve.OnlyLook:
            case PlayerAchieve.FlashLockOnlyLook:
                break;
        }
        _scenarioManager.SetAchieve(playerAchieve,state);
        if (state)
        {
            playerSettings[playerAchieve]?.Invoke();
        }
        else
        {
            playerSettings[PlayerAchieve.PlayerMove]?.Invoke();
            _gameManager?.Player?.PlayerSkillEffect?.SkillCanvasActiveTrue();
        }
    }
    private void PlayerMove()
    {
        // 플레이어가 Null 일 경우 실행 오류를 방지한다.
        if (_gameManager.Player is null) return;
        
        _gameManager.Player.PlayerSetting(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void PlayerStop()
    {
        _gameManager.Player.PlayerSetting(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }
    private void InteractOrCutScene()
    {
        _gameManager.Player.PlayerSetting(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _gameManager.Player.PlayerSkillEffect.SkillCanvasActiveFalse();
        _gameManager.Player.OffSkillEvent();
    }
    private void DialogueOrSavingOrMenu()
    {
        _gameManager.Player.PlayerSetting(false);
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;
        _gameManager.Player.PlayerSkillEffect.SkillCanvasActiveFalse();
        _gameManager.Player.OffSkillEvent();
    }
    private void Menu()
    {
        _gameManager.Player.PlayerSetting(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _gameManager.Player.PlayerSkillEffect.SkillCanvasActiveFalse();
    }
    private void LookAndInteract()
    {
        _gameManager.Player.PlayerSetting(false,true,false,true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }
    private void InteractLock()
    {
        _gameManager.Player.PlayerSetting(true,false,true,true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnlyLook()
    {
        _gameManager.Player.PlayerSetting(false,false,true,true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void FlashLockOnlyLook()
    {
        _gameManager.Player.PlayerSetting(false,false,false,true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PlayerSkillValue(bool value) { _gameManager.Player.SkillSetting(value); }

    public void ResetBool()
    {
        var e = new[]
        {
            PlayerAchieve.EscMenu,
            PlayerAchieve.CutScenePlaying,
            PlayerAchieve.ObjectInteract,
            PlayerAchieve.OnlyInteractLocked,
            PlayerAchieve.LookAndInteract,
            PlayerAchieve.Dialogue,
            PlayerAchieve.Saving,
            PlayerAchieve.PlayerMove,
            PlayerAchieve.OnlyLook,
            PlayerAchieve.PlayerStop,
            PlayerAchieve.FlashLockOnlyLook,
        };

        foreach (var a in e)
        {
            if (_scenarioManager.GetAchieve(a))
            {
                _scenarioManager.SetAchieve(a,false);
            }
        }
    }
}
