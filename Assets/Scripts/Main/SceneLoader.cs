using ConnectFourMultiplayer.Utilities;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ConnectFourMultiplayer.Main
{
    public class SceneLoader : GenericMonoSingleton<SceneLoader>
    {
        protected override void Awake()
        {
            base.Awake();
        }

        public void LoadScene(SceneNameEnum sceneName, bool isNetworked, LoadSceneMode mode = LoadSceneMode.Single)
        {
            if (isNetworked)
            {
                //if (NetworkManager.Singleton == null) return;

                //if (!NetworkManager.Singleton.IsServer) return;

                //NetworkManager.Singleton.SceneManager.LoadScene(sceneName.ToString(), mode);
            }
            else
            {
                StartCoroutine(LoadSceneAsyncCoroutine(sceneName, mode));
            }
        }

        private IEnumerator LoadSceneAsyncCoroutine(SceneNameEnum sceneName, LoadSceneMode mode)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName.ToString(), mode);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }
}