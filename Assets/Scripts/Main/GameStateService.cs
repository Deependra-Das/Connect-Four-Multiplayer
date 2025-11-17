using ConnectFourMultiplayer.Event;

namespace ConnectFourMultiplayer.Main
{
    public class GameStateService
    {
        private GameStateEnum _currentState = GameStateEnum.MainMenu;
        public GameStateEnum CurrentState => _currentState;

        public void ChangeState(GameStateEnum newState)
        {
            if (_currentState == newState) return;

            _currentState = newState;
            EventBusManager.Instance.Raise(EventNameEnum.ChangeGameState, _currentState);
        }
    }
}