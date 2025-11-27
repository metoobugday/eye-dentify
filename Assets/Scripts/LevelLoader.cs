using System.Collections;
using System; // Action kullanabilmek için gerekli
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public static LevelLoader instance;
    public Animator transitionAnimator;
    public float transitionTime = 1f; // Inspector'dan 1 veya 2 yapmayı unutma!

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ARTIK BU FONKSİYONU KULLANACAĞIZ
    // Bu fonksiyon içine "Ne Yapılacağını" (action) parametre olarak alır.
    public void PlayTransition(Action actionToPerform)
    {
        StartCoroutine(TransitionRoutine(actionToPerform));
    }

    IEnumerator TransitionRoutine(Action action)
    {
        // 1. Gözleri Kapat
        transitionAnimator.SetTrigger("Close");

        // 2. Animasyon süresi kadar bekle
        yield return new WaitForSeconds(transitionTime);

        // 3. İstenilen işlemi yap (Sahne yükle veya Menü aç)
        // Burası sihirli kısımdır, ne emir verdiysen onu yapar.
        action?.Invoke();

        // 4. Gözleri Aç
        transitionAnimator.SetTrigger("Open");
    }
}