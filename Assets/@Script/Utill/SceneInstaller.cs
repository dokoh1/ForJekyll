using Zenject;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SceneInstaller : MonoInstaller
{
    [Inject]private Dictionary<SceneEnum, Type> _sceneDictionary;
    [SerializeField] private SceneEnum currentScene;
    public override void InstallBindings()
    {
        if (_sceneDictionary.TryGetValue(currentScene, out Type managerType))
        {
            // ✅ Zenject 컨테이너를 통해 생성 (AddComponent 대신 사용)
            Container.Bind(managerType).FromNewComponentOnNewGameObject().AsSingle();
        }
        else
        {
            Debug.LogError($"[SceneInstaller] {currentScene}에 해당하는 Scene Manager가 없습니다.");
        }
    }
}