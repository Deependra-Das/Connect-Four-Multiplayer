using UnityEngine;

public class GameplayManager : GenericMonoSingleton<GameplayManager>
{
    [SerializeField] private Transform[] _spawnLocations;

    public void TakeTurn(int colIndex)
    {
        GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskRed, _spawnLocations[colIndex].position);
    }
}
