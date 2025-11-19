using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string gameSceneName = "Level1";    // Oyun sahnenin adı (ör: Level1, Scene_Gameplay)
    public string mainMenuSceneName = "MainMenu"; // Ana menü sahnenin adı

    // GameOver sahnesindeyken "Restart" butonundan çağrılacak
    public void OnRestartButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    // "Main Menu" butonundan çağrılacak
    public void OnMainMenuButton()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // "Quit" butonundan çağrılacak
    public void OnQuitButton()
    {
        // Editor'de çalışmaz, sadece build'de oyunu kapatır
        Application.Quit();

        // Editor'de test etmek için:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
