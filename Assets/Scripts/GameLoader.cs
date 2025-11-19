using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoader : MonoBehaviour
{
    private void Start()
    {
        // Persistent sahne zaten varsa tekrar yükleme
        if (!IsSceneLoaded("PersistentPauseUI"))
        {
            SceneManager.LoadScene("PersistentPauseUI", LoadSceneMode.Additive);
            Debug.Log("[PersistentPauseUI] Loaded additively.");
        }
        else
        {
            Debug.Log("[PersistentPauseUI] Already loaded, skipping reload.");
        }
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
                return true;
        }
        return false;
    }
}
