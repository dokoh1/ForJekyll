using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Dictionary<SceneEnum, Type> scenes = new Dictionary<SceneEnum, Type>()
        {
            {SceneEnum.Tutorial, typeof(TutorialManager)},
            {SceneEnum.Chapter1_1, typeof(Chapter1_1_Manager)},
            {SceneEnum.Chapter1_2, typeof(Chapter1_2_Manager)},
            {SceneEnum.Chapter2_1, typeof(Chapter2_1_Manager)},
            {SceneEnum.Chapter2_3, typeof(Chapter2_3_Manager)},
            {SceneEnum.Chapter3_1, typeof(Chapter3_1_Manager)}, 
            {SceneEnum.Chapter3_2, typeof(Chapter3_2_Manager)},
            {SceneEnum.Chapter4_1, typeof(Chapter4_1_Manager)},
            {SceneEnum.Chapter4_2, typeof(Chapter4_2_Manager)},
        };
        Container.Bind<GameManager>().FromInstance(FindAnyObjectByType<GameManager>()).AsSingle();
        Container.Bind<SoundManager>().FromInstance(FindAnyObjectByType<SoundManager>()).AsSingle();
        Container.Bind<DataManager>().FromInstance(FindAnyObjectByType<DataManager>()).AsSingle();
        Container.Bind<UIManager>().FromInstance(FindAnyObjectByType<UIManager>()).AsSingle();
        Container.Bind<ScenarioManager>().FromInstance(FindAnyObjectByType<ScenarioManager>()).AsSingle();
        Container.Bind<UI_PauseMenuPopup>().FromInstance(FindAnyObjectByType<UI_PauseMenuPopup>()).AsSingle();
        
        Container.BindInstance(scenes).AsSingle();
    }
}


