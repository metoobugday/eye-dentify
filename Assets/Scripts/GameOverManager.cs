using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameOverSceneName = "GameOver";

    public void GameOver()
    {
        SceneManager.LoadScene(gameOverSceneName);
    }
}
