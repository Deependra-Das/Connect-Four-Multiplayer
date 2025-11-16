using ConnectFourMultiplayer.Event;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConnectFourMultiplayer.Main
{
    public class GameStateManager : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(InitializeAfterSceneLoad());
        }

        private IEnumerator InitializeAfterSceneLoad()
        {
            yield return null;
            string activeSceneName = SceneManager.GetActiveScene().name;

            if (!Enum.TryParse(activeSceneName, out SceneNameEnum sceneName))
            {
                Debug.LogWarning($"[GameStateManager] Scene '{activeSceneName}' does not match any SceneName enum.");
                yield break;
            }

            EventBusManager.Instance.Raise(EventNameEnum.SceneLoaded, sceneName);
        }
    }
}
