using ConnectFourMultiplayer.Event;
using System;
using UnityEngine;

namespace ConnectFourMultiplayer.Main
{
    public class GameStateService
    {
        private GameStateEnum _currentState = GameStateEnum.MainMenu;
        public GameStateEnum CurrentState => _currentState;

        public void ChangeState(GameStateEnum newState)
        {
            if (_currentState == newState)
                return;
            _currentState = newState;
            HandleSceneChange(newState);
        }

        private void HandleSceneChange(GameStateEnum newState)
        {
            switch (newState)
            {
                case GameStateEnum.MainMenu:
                    SceneLoader.Instance.LoadScene(SceneNameEnum.MainMenuScene, false);
                    break;
                case GameStateEnum.Lobby:
                    SceneLoader.Instance.LoadScene(SceneNameEnum.LobbyScene, false);
                    break;
                case GameStateEnum.Gameplay:
                    SceneLoader.Instance.LoadScene(SceneNameEnum.GameplayScene, false);
                    break;
                case GameStateEnum.GameOver:
                    SceneLoader.Instance.LoadScene(SceneNameEnum.GameOverScene, false);
                    break;
            }

            EventBusManager.Instance.Raise(EventNameEnum.ChangeGameState, _currentState);
        }
    }
}