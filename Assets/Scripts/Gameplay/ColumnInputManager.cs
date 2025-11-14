using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Network;
using System.Collections;
using UnityEngine;

namespace ConnectFourMultiplayer.Gameplay
{
    public class ColumnInputManager : MonoBehaviour
    {
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private int _columnIndex;
        [SerializeField] private float _cooldownTime = 3f;


        private void OnEnable()
        {
            EventBusManager.Instance.Subscribe(EventNameEnum.EnableColumnInput, HandleEnableColumnInput);
            EventBusManager.Instance.Subscribe(EventNameEnum.GameOver, HandleGameOverColumnInput);
        }

        private void OnDisable()
        {
            EventBusManager.Instance.Unsubscribe(EventNameEnum.TakeTurn, HandleEnableColumnInput);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.GameOver, HandleGameOverColumnInput);
        }

        private void OnMouseDown()
        {
            bool turnSuccess = GameplayManager.Instance.TryTakeTurn(_columnIndex);
            SetCollidersState(!turnSuccess);
        }

        //private void OnMouseOver()
        //{
        //    if (!_cooldownActive && !_isGameOVer)
        //    {
        //        GameplayManager.Instance.OnHoverOverColumn(_columnIndex);
        //    }
        //}

        private void HandleEnableColumnInput(object[] parameters)
        {
            SetCollidersState(true);
        }

        private void SetCollidersState(bool state)
        {
            boxCollider.enabled = state;
        }

        private void HandleGameOverColumnInput(object[] parameters)
        {
            SetCollidersState(false);
        }
    }
}