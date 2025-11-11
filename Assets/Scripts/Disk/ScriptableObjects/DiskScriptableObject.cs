using UnityEngine;

namespace ConnectFourMultiplayer.Disk
{
    [CreateAssetMenu(fileName = "DiskScriptableObject", menuName = "ScriptableObjects/DiskScriptableObject")]
    public class DiskScriptableObject : ScriptableObject
    {
        public GameObject diskRedPrefab;
        public GameObject diskYellowPrefab;
        public GameObject diskRedPreviewPrefab;
        public GameObject diskYellowPreviewPrefab;
    }
}