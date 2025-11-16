using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Gameplay;
using UnityEngine;
using UnityEngine.UIElements;

namespace ConnectFourMultiplayer.Disk
{
    public class DiskPreviewService
    {
        private GameObject _diskRedPreviewPrefab;
        private GameObject _diskYellowPreviewPrefab;

        private GameObject _diskRedPreview;
        private GameObject _diskYellowPreview;

        public DiskPreviewService(DiskScriptableObject diskScriptableObject)
        {
            _diskRedPreviewPrefab = diskScriptableObject.diskRedPreviewPrefab;
            _diskYellowPreviewPrefab = diskScriptableObject.diskYellowPreviewPrefab;
        }

        ~DiskPreviewService()
        {
            _diskRedPreviewPrefab = null;
            _diskYellowPreviewPrefab = null;
            _diskRedPreview = null;
            _diskYellowPreview = null;
        }

        public void Initialize(Vector3 spawnLocation)
        {
            _diskRedPreview = GameObject.Instantiate(_diskRedPreviewPrefab, spawnLocation, Quaternion.identity);
            _diskYellowPreview = GameObject.Instantiate(_diskYellowPreviewPrefab, spawnLocation, Quaternion.identity);

            ToggleDiskRedPreview(false);
            ToggleDiskYellowPreview(false);
        }

        private void ToggleDiskRedPreview(bool visibility)
        {
            if (_diskRedPreview != null)
            {
                _diskRedPreview.SetActive(visibility);
            }           
        }

        private void ToggleDiskYellowPreview(bool visibility)
        {
            if (_diskYellowPreview != null)
            {
                _diskYellowPreview.SetActive(visibility);
            }
        }

        public void HandleHoverOverColumnDiskPreview(PlayerTurnEnum playerTurn, Vector3 position)
        {
            switch (playerTurn)
            {
                case PlayerTurnEnum.Player1:
                    _diskRedPreview.transform.position = position;
                    break;
                case PlayerTurnEnum.Player2:
                    _diskYellowPreview.transform.position = position;
                    break;
            }
        }

        public void DisableDiskPreview()
        {
            ToggleDiskRedPreview(false);
            ToggleDiskYellowPreview(false);
        }

        public void HandlePlayerTurnChangeDiskPreview(PlayerTurnEnum playerTurn)
        {
            switch (playerTurn)
            {
                case PlayerTurnEnum.Player1:
                    ToggleDiskRedPreview(true);
                    ToggleDiskYellowPreview(false);
                    break;
                case PlayerTurnEnum.Player2:
                    ToggleDiskRedPreview(false);
                    ToggleDiskYellowPreview(true);
                    break;
            }
        }

        public void Reset()
        {
            DisableDiskPreview();

            if (_diskRedPreview != null)
            {
                GameObject.Destroy(_diskRedPreview);
            }
            if (_diskYellowPreview != null)
            {
                GameObject.Destroy(_diskYellowPreview);
            }
        }
    }
}