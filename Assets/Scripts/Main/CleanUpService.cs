using ConnectFourMultiplayer.Board;
using ConnectFourMultiplayer.Disk;
using ConnectFourMultiplayer.LobbyRelay;
using ConnectFourMultiplayer.Network;
using Unity.Netcode;
using UnityEngine;

namespace ConnectFourMultiplayer.Main
{
    public class CleanUpService
    {
        public CleanUpService() { }

        public void CleanUpPlayerSessionDataManager()
        {
            CleanUpNetworkObject(PlayerSessionDataManager.Instance);
        }

        public void CleanUpMultiplayerManager()
        {
            CleanUpNetworkObject(MultiplayerManager.Instance);
        }

        public void CleanUpNetworkManager()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
                UnityEngine.Object.Destroy(NetworkManager.Singleton.gameObject);
            }
        }

        private void CleanUpNetworkObject(MonoBehaviour obj)
        {
            if (obj != null)
            {
                if (obj is NetworkBehaviour networkBehaviour)
                {
                    var networkObject = networkBehaviour.GetComponent<NetworkObject>();
                    if (networkObject != null && networkObject.IsSpawned && NetworkManager.Singleton.IsServer)
                    {
                        networkObject.Despawn(false);
                    }
                }
                UnityEngine.Object.Destroy(obj.gameObject);
            }
        }

        public void CleanUpLobbyRelay()
        {
            if (LobbyRelayManager.Instance != null)
            {
                UnityEngine.Object.Destroy(LobbyRelayManager.Instance.gameObject);
            }
        }

        public void ResetServices()
        {
            if (GameManager.Instance != null)
            {
                var boardService = GameManager.Instance.Get<BoardService>();
                if (boardService != null)
                {
                    boardService.Reset();
                }

                var diskPreviewService = GameManager.Instance.Get<DiskPreviewService>();
                if (diskPreviewService != null)
                {
                    diskPreviewService.Reset();
                }
            }
        }
    }

}
