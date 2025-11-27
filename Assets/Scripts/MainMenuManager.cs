using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager Instance;
    [SerializeField] private bool _debugMode;
    public enum MainMenuButtons { play, options, credits, quit };
    public enum CreditsButtons { back };
    public enum SettingsButtons { back };

    [SerializeField] GameObject _MainMenuContainer;
    [SerializeField] GameObject _CreditsMenuContainer;
    [SerializeField] GameObject _SettingsMenuContainer;

    [SerializeField] private string _sceneToLoadAfterClickingPlay;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There are more than 1 MainMenuManager's in the scene");
        }
    }

    private void Start()
    {
        // Oyun ilk açıldığında direkt menüyü göster (animasyonsuz)
        OpenMenu(_MainMenuContainer);
    }

    public void MainMenuButtonClicked(MainMenuButtons buttonClicked)
    {
        DebugMessage("Button Clicked: " + buttonClicked.ToString());

        // ÖNEMLİ DEĞİŞİKLİK BURADA:
        // İşlemleri direkt yapmak yerine LevelLoader'a "Geçiş Yap ve Sonra Bunu Çalıştır" diyoruz.

        switch (buttonClicked)
        {
            case MainMenuButtons.play:
                // Play'e basınca: Göz kapa -> Sahne Yükle -> Göz Aç
                LevelLoader.instance.PlayTransition(() => PlayClicked());
                break;

            case MainMenuButtons.options:
                // Options'a basınca: Göz kapa -> Options Menüsünü Aç -> Göz Aç
                LevelLoader.instance.PlayTransition(() => OptionsClicked());
                break;

            case MainMenuButtons.credits:
                LevelLoader.instance.PlayTransition(() => CreditsClicked());
                break;

            case MainMenuButtons.quit:
                LevelLoader.instance.PlayTransition(() => QuitGame());
                break;

            default:
                Debug.Log("Button clicked that wasn't implemented");
                break;
        }
    }

    // --- Alt Menülerden Geri Dönüş Butonları ---

    public void CreditsButtonClicked(CreditsButtons buttonClicked)
    {
        switch (buttonClicked)
        {
            case CreditsButtons.back:
                // Geri tuşuna basınca da efekt olsun
                LevelLoader.instance.PlayTransition(() => ReturnToMainMenu());
                break;
        }
    }

    public void SettingsButtonClicked(SettingsButtons buttonClicked)
    {
        switch (buttonClicked)
        {
            case SettingsButtons.back:
                LevelLoader.instance.PlayTransition(() => ReturnToMainMenu());
                break;
        }
    }

    // --- Temel Fonksiyonlar (Aynı Kalabilir veya sadeleşebilir) ---

    public void CreditsClicked()
    {
        OpenMenu(_CreditsMenuContainer);
    }

    public void OptionsClicked()
    {
        OpenMenu(_SettingsMenuContainer);
    }

    public void ReturnToMainMenu()
    {
        OpenMenu(_MainMenuContainer);
    }

    public void PlayClicked()
    {
        SceneManager.LoadScene(_sceneToLoadAfterClickingPlay);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
    }

    // Menü açma kapama mantığı aynen kalıyor
    public void OpenMenu(GameObject menuToOpen)
    {
        _MainMenuContainer.SetActive(menuToOpen == _MainMenuContainer);
        _CreditsMenuContainer.SetActive(menuToOpen == _CreditsMenuContainer);
        _SettingsMenuContainer.SetActive(menuToOpen == _SettingsMenuContainer);
    }

    private void DebugMessage(string message)
    {
        if (_debugMode)
        {
            Debug.Log(message);
        }
    }
}