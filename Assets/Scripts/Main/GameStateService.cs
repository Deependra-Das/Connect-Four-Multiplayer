using ConnectFourMultiplayer.Event;

namespace ConnectFourMultiplayer.Main
{
    public class GameStateService
    {
        private GameStateEnum _currentState = GameStateEnum.MainMenu;
        public GameStateEnum CurrentState => _currentState;

        public GameStateService()
        {
            EventBusManager.Instance.Subscribe(EventNameEnum.SceneLoaded, OnSceneLoaded);
        }

        ~GameStateService()
        {
            EventBusManager.Instance.Unsubscribe(EventNameEnum.SceneLoaded, OnSceneLoaded);
        }

        private void OnSceneLoaded(object[] paramters)
        {
            if (paramters == null || paramters.Length == 0)
                return;

            SceneNameEnum currentSceneLoaded = (SceneNameEnum)paramters[0];
            HandleSceneGameStateChange(currentSceneLoaded);
        }

        private void HandleSceneGameStateChange(SceneNameEnum currentScene)
        {
            switch (currentScene)
            {
                case SceneNameEnum.MainMenuScene:
                    ChangeState(GameStateEnum.MainMenu);
                    break;
                case SceneNameEnum.LobbyScene:
                    ChangeState(GameStateEnum.Lobby);
                    break;
                case SceneNameEnum.GameplayScene:
                    ChangeState(GameStateEnum.Gameplay);
                    break;
                case SceneNameEnum.GameOverScene:
                    ChangeState(GameStateEnum.GameOver);
                    break;
            }
        }

        public void ChangeState(GameStateEnum newState)
        {
            if (_currentState == newState) return;

            _currentState = newState;
            EventBusManager.Instance.Raise(EventNameEnum.ChangeGameState, _currentState);
        }


    }
}