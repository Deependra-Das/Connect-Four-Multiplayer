using ConnectFourMultiplayer.Main;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameOverUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _winnerUserNameText;
    [SerializeField] private TMP_Text _gameOverCountdownText;
    [SerializeField] private float _gameOverCountdownValue = 5f;    

    void Start()
    {
        EnableView();
        StartCoroutine(GameOverCountdownSequence());
    }

    public void EnableView()
    {
        gameObject.SetActive(true);
    }

    public void DisableView()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator GameOverCountdownSequence()
    {
        float currentTime = _gameOverCountdownValue;

        while (currentTime > 0)
        {
            _gameOverCountdownText.text = Mathf.Ceil(currentTime).ToString() + "s";
            currentTime -= 1f;
            yield return new WaitForSeconds(1f);
        }

        SceneLoader.Instance.LoadScene(SceneNameEnum.MainMenuScene, false);
    }
}
