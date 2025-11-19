using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject optionsMenuUI;

    private bool _isPaused = false;

    private void Awake()
    {
        // Eğer başka bir kopya varsa sil, yoksa bu kopyayı koru
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 🎯 Level değişse de bu obje kalır
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Başlangıçta menüler kapalı başlar
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (optionsMenuUI != null)
            optionsMenuUI.SetActive(false);
    }

    private void Update()
    {
        // ESC tuşuyla pause menüyü aç/kapat
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused)
                Resume();
            else
                Pause();
        }
    }

    // 🔹 Oyuna devam et
    public void Resume()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        _isPaused = false;
    }

    // 🔹 Oyunu durdur
    public void Pause()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        _isPaused = true;
    }

    // 🔹 Options Menüsünü Aç
    public void OpenOptions()
    {
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        if (optionsMenuUI != null)
            optionsMenuUI.SetActive(true);
    }

    // 🔹 Options’tan geri dön
    public void CloseOptions()
    {
        if (optionsMenuUI != null)
            optionsMenuUI.SetActive(false);

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);
    }

    // 🔹 Ana menüye dön
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void ForceClose()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameOver")
        {
            // Gameplay UI’yı kapat
            GameObject gameUI = GameObject.Find("GameUI");  // kendi UI objenin adı
            if (gameUI != null)
                gameUI.SetActive(false);

            // Pause kontrolcüsünü sil
            Destroy(gameObject);
            Instance = null;
        }

        else if (scene.name == "MainMenu")
        {
            Destroy(gameObject);
            Instance = null;
        }
    }

    // 🔹 Oyunu kapat
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
           Application.Quit();
#endif
    }
}


