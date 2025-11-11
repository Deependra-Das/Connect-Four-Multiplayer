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

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            RegisterServices();
            ServiceLocator.Get<GameStateService>().ChangeState(GameStateEnum.MainMenu);
        }

        private void RegisterServices()
        {
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
        }

        private void OnDestroy()
        {
            DeregisterServices();
        }

        public T Get<T>()
        {
            return ServiceLocator.Get<T>();
        }
    }
}