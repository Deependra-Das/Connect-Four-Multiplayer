using ConnectFourMultiplayer.Event;
using System.Collections;
using UnityEngine;

namespace ConnectFourMultiplayer.Gameplay
{
    public class ColumnInputManager : MonoBehaviour
    {
        [SerializeField] private BoxCollider boxCollider;
        [SerializeField] private int _columnIndex;
        [SerializeField] private float _cooldownTime = 3f;
        private bool _cooldownActive = false;

        private void OnEnable()
        {
            EventBusManager.Instance.Subscribe(EventNameEnum.TakeTurn, HandleTakeTurnColumnInput);
        }

        private void OnDisable()
        {
            EventBusManager.Instance.Unsubscribe(EventNameEnum.TakeTurn, HandleTakeTurnColumnInput);
        }

        private void OnMouseDown()
        {
            if (!_cooldownActive)
            {
                _cooldownActive = true;
                SetCollidersState(false);
                GameplayManager.Instance.TakeTurn(_columnIndex);
            }
        }

        private void OnMouseOver()
        {
            if (!_cooldownActive)
            {
                GameplayManager.Instance.OnHoverOverColumn(_columnIndex);
            }
        }

        private void HandleTakeTurnColumnInput(object[] parameters)
        {
            StartCoroutine(CooldownForAllObjects(_cooldownTime));
        }

        private IEnumerator CooldownForAllObjects(float cooldownTime)
        {
            SetCollidersState(false);

            yield return new WaitForSeconds(cooldownTime);

            SetCollidersState(true);

            _cooldownActive = false;
        }

        private void SetCollidersState(bool state)
        {
            {
                GetComponent<Collider>().enabled = state;
            }
        }
    }
}