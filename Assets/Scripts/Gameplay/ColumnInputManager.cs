using UnityEngine;

public class ColumnInputManager : MonoBehaviour
{

    [SerializeField] private int _columnIndex;


    private void OnMouseDown()
    {
        GameplayManager.Instance.TakeTurn(_columnIndex);
    }
}
