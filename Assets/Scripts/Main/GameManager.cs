using ConnectFourMultiplayer.Board;
using ConnectFourMultiplayer.Disk;
using ConnectFourMultiplayer.Utilities;
using UnityEngine;

namespace ConnectFourMultiplayer.Main
{
    public class GameManager : GenericMonoSingleton<GameManager>
    {
        [SerializeField] private DiskScriptableObject _disk_SO;
        [SerializeField] private BoardScriptableObject _board_SO;

        public const string UsernameKey = "Username";


        private void Start()
        {
            RegisterServices();
            ServiceLocator.Get<GameStateService>().ChangeState(GameStateEnum.MainMenu);
        }

        private void RegisterServices()
        {
            ServiceLocator.Register(new CleanUpService());
            ServiceLocator.Register(new GameStateService());
            ServiceLocator.Register(new DiskSpawnService(_disk_SO));
            ServiceLocator.Register(new BoardService(_board_SO));
            ServiceLocator.Register(new DiskPreviewService(_disk_SO));
        }

        private void DeregisterServices()
        {
            ServiceLocator.Unregister<DiskSpawnService>();
            ServiceLocator.Unregister<BoardService>();
            ServiceLocator.Unregister<DiskPreviewService>();
            ServiceLocator.Unregister<GameStateService>();
            ServiceLocator.Unregister<CleanUpService>();
        }

        public T Get<T>()
        {
            return ServiceLocator.Get<T>();
        }
    }
}