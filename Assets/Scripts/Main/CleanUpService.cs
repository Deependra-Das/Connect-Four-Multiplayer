using ConnectFourMultiplayer.Event;
using ConnectFourMultiplayer.Network;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConnectFourMultiplayer.Main
{
    public class CleanUpService
    {
        public CleanUpService() 
        {
            EventBusManager.Instance.Subscribe(EventNameEnum.ChangeGameState, HandleCleanUp);
        }

        ~CleanUpService() 
        {
            EventBusManager.Instance.Unsubscribe(EventNameEnum.ChangeGameState, HandleCleanUp);
        }

        private void HandleCleanUp(object[] parameters)
        {
            string activeSceneName = SceneManager.GetActiveScene().name.ToString();
            Enum.TryParse<SceneNameEnum>(activeSceneName, out var sceneEnumValue);

            switch (sceneEnumValue)
            {
                case SceneNameEnum.GameOverScene:
                    GameOverCleanup();
                    break;                
            }
        }

        public void GameOverCleanup()
        {
            if (GameplayManager.Instance != null)
            {
                UnityEngine.Object.Destroy(GameplayManager.Instance.gameObject);
            }
        }
    }
}
