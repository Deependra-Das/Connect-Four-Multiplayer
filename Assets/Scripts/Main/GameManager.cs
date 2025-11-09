using UnityEngine;

public class GameManager : GenericMonoSingleton<GameManager>
{
    [SerializeField] private DiskScriptableObject _disk_SO;
    [SerializeField] private BoardScriptableObject _board_SO;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        RegisterServices();
        GameplayManager.Instance.Initialize();
    }

    private void RegisterServices()
    {
        ServiceLocator.Register(new DiskSpawnService(_disk_SO));
        ServiceLocator.Register(new BoardService(_board_SO));
    }

    private void DeregisterServices()
    {

    }

    public T Get<T>()
    {
        return ServiceLocator.Get<T>();
    }
}