using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Main;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace ConnectFourMultiplayer.Network
{
    public class GameOverManager : NetworkBehaviour
    {
        private bool winnerDetailsSet = false;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if (IsServer)
            {
                StartCoroutine(GameOverSequence());
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
        }

        private IEnumerator GameOverSequence()
        {
            SetWinnerDetailsServerRpc();
            yield return new WaitUntil(() => winnerDetailsSet);
            StartCountdownServerRpc();
        }


        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void SetWinnerDetailsServerRpc()
        {
            SetWinnerDetailsClientRpc();
        }

        [ClientRpc]
        private void SetWinnerDetailsClientRpc()
        {
            EventBusManager.Instance.RaiseNoParams(EventNameEnum.SetWinnerOnGameOverUI);
            ConfirmWinnerDetailsSetServerRpc();
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void ConfirmWinnerDetailsSetServerRpc()
        {
            winnerDetailsSet = true;
        }

        [Rpc(SendTo.Server, InvokePermission = RpcInvokePermission.Everyone)]
        private void StartCountdownServerRpc()
        {
            StartCountdownClientRpc();
        }

        [ClientRpc]
        private void StartCountdownClientRpc()
        {
            EventBusManager.Instance.RaiseNoParams(EventNameEnum.StartGameOverCountdown);
            CleanUp();
        }

        private void CleanUp()
        {
            GameManager.Instance.Get<CleanUpService>().CleanUpPlayerSessionDataManager();
            GameManager.Instance.Get<CleanUpService>().CleanUpLobbyRelay();
            GameManager.Instance.Get<CleanUpService>().ResetServices();
        }
    }
}