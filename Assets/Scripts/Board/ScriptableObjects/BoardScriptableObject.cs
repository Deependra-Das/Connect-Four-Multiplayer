using UnityEngine;

[CreateAssetMenu(fileName = "BoardScriptableObject", menuName = "ScriptableObjects/BoardScriptableObject")]
public class BoardScriptableObject : ScriptableObject
{
    public int boardRowCount;
    public int boardColumnCount;
}
