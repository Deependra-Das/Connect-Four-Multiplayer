using UnityEngine;

public class GameplayManager : GenericMonoSingleton<GameplayManager>
{
    [SerializeField] private Transform[] _spawnLocations;

    bool playerTurn = false;

    public void TakeTurn(int colIndex)
    {
        switch (playerTurn)
        {
            case false:
                GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskRed, _spawnLocations[colIndex].position);
                break;
            case true:
                GameManager.Instance.Get<DiskSpawnService>().SpawnDisk(DiskTypeEnum.DiskYellow, _spawnLocations[colIndex].position);
                break;
        }

        playerTurn = !playerTurn;
    }
}
