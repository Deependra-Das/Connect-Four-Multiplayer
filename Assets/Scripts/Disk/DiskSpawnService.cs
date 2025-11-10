using UnityEngine;

public class DiskSpawnService
{
    private DiskScriptableObject _disk_SO;

    public DiskSpawnService(DiskScriptableObject diskScriptableObject)
    {
        _disk_SO = diskScriptableObject;
    }
    ~DiskSpawnService()
    {
        _disk_SO = null;
    }

    public GameObject SpawnDisk(DiskTypeEnum diskType, Vector3 spawnLocation)
    {
        GameObject newDisk;
        
        switch(diskType)
        {
            case DiskTypeEnum.DiskRed:
                newDisk = GameObject.Instantiate(_disk_SO.diskRedPrefab, spawnLocation, Quaternion.identity);
                break;
            case DiskTypeEnum.DiskYellow:
                newDisk = GameObject.Instantiate(_disk_SO.diskYellowPrefab, spawnLocation, Quaternion.identity);
                break;
            default:
                newDisk = null;
                Debug.LogError("Unhandled disk type: " + diskType);
                break;
        }

        return newDisk;       
    }
}
