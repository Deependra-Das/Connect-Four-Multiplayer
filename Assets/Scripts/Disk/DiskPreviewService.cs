using System;
using UnityEngine;
using UnityEngine.UIElements;

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
        SubscribeToEvents();
    }

    ~DiskPreviewService()
    {
        UnsubscribeToEvents();
        _diskRedPreviewPrefab = null;
        _diskYellowPreviewPrefab = null;
        _diskRedPreview = null;
        _diskYellowPreview = null;
    }

    private void SubscribeToEvents()
    {
        EventBusManager.Instance.Subscribe(EventNameEnum.TakeTurn, HandleTakeTurnDiskPreview);
    }

    private void UnsubscribeToEvents()
    {
        EventBusManager.Instance.Unsubscribe(EventNameEnum.TakeTurn, HandleTakeTurnDiskPreview);
    }

    public void Initialize(Vector3 spawnLocation)
    {
        _diskRedPreview = GameObject.Instantiate(_diskRedPreviewPrefab, spawnLocation, Quaternion.identity);
        _diskYellowPreview =  GameObject.Instantiate(_diskYellowPreviewPrefab, spawnLocation, Quaternion.identity);

        ToggleDiskRedPreview(false);
        ToggleDiskYellowPreview(false);
    }

    private void ToggleDiskRedPreview(bool visibility)
    {
        _diskRedPreview.SetActive(visibility);
    }

    private void ToggleDiskYellowPreview(bool visibility)
    {
        _diskYellowPreview.SetActive(visibility);
    }

    public void HandleHoverOverColumnDiskPreview(PlayerTurnEnum playerTurn, Vector3 position)
    {
        switch (playerTurn)
        {
            case PlayerTurnEnum.Player1:
                ToggleDiskRedPreview(true);
                ToggleDiskYellowPreview(false);
                _diskRedPreview.transform.position = position;
                break;
            case PlayerTurnEnum.Player2:
                ToggleDiskRedPreview(false);
                ToggleDiskYellowPreview(true);
                _diskYellowPreview.transform.position = position;
                break;
        }
    }

    private void HandleTakeTurnDiskPreview(object[] parameters)
    {
        ToggleDiskRedPreview(false);
        ToggleDiskYellowPreview(false);
    }
}
