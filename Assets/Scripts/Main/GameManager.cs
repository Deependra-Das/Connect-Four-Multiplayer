using UnityEngine;

public class GameManager : GenericMonoSingleton<GameManager>
{
    [SerializeField] private DiskScriptableObject _disk_SO;

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        RegisterServices();
    }

    private void RegisterServices()
    {
        ServiceLocator.Register(new DiskSpawnService(_disk_SO));
    }

    private void DeregisterServices()
    {

    }

    public T Get<T>()
    {
        return ServiceLocator.Get<T>();
    }
}