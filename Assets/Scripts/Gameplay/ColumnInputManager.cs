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

        private void OnEnable()
        {
            EventBusManager.Instance.Subscribe(EventNameEnum.EnableColumnInput, HandleEnableColumnInput);
            EventBusManager.Instance.Subscribe(EventNameEnum.GameOver, HandleGameOverColumnInput);
            EventBusManager.Instance.Subscribe(EventNameEnum.DisableColumnInput, HandleDisableColumnInput);
        }

        private void OnDisable()
        {
            EventBusManager.Instance.Unsubscribe(EventNameEnum.EnableColumnInput, HandleEnableColumnInput);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.GameOver, HandleGameOverColumnInput);
            EventBusManager.Instance.Unsubscribe(EventNameEnum.DisableColumnInput, HandleDisableColumnInput);
        }

        private void OnMouseDown()
        {
            bool turnSuccess = GameplayManager.Instance.TryTakeTurn(_columnIndex);
            SetCollidersState(!turnSuccess);
        }

        private void OnMouseOver()
        {
            if (boxCollider.enabled)
            {
                GameplayManager.Instance.OnHoverOverColumn(_columnIndex);
            }
        }

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

        private void HandleDisableColumnInput(object[] parameters)
        {
            SetCollidersState(false);
        }
    }
}