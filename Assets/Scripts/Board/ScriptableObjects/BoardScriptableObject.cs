using UnityEngine;

namespace ConnectFourMultiplayer.Board
{
    [CreateAssetMenu(fileName = "BoardScriptableObject", menuName = "ScriptableObjects/BoardScriptableObject")]
    public class BoardScriptableObject : ScriptableObject
    {
        public int boardRowCount;
        public int boardColumnCount;
    }
}