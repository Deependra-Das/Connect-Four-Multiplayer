using ConnectFourMultiplayer.Event;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConnectFourMultiplayer.Main
{
    public class GameStateManager : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(InitializeAfterSceneLoad());
        }

        private IEnumerator InitializeAfterSceneLoad()
        {
            yield return null;
            string activeSceneName = SceneManager.GetActiveScene().name;

            if (!Enum.TryParse(activeSceneName, out SceneNameEnum sceneName))
            {
                Debug.LogWarning($"[GameStateManager] Scene '{activeSceneName}' does not match any SceneName enum.");
                yield break;
            }

            GameManager.Instance.Get<GameStateService>().ChangeState(HandleSceneToGameStateConversion(sceneName));            
        }

        private GameStateEnum HandleSceneToGameStateConversion(SceneNameEnum currentScene)
        {
            GameStateEnum newState = GameStateEnum.MainMenu;
            switch (currentScene)
            {
                case SceneNameEnum.MainMenuScene:
                    newState = GameStateEnum.MainMenu;
                    break;
                case SceneNameEnum.LobbyScene:
                    newState = GameStateEnum.Lobby;
                    break;
                case SceneNameEnum.GameplayScene:
                    newState = GameStateEnum.Gameplay;
                    break;
                case SceneNameEnum.GameOverScene:
                    newState = GameStateEnum.GameOver;
                    break;
            }
            return newState;
        }
    }
}
