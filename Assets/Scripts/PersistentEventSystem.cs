using UnityEngine;
using UnityEngine.EventSystems;

public class PersistentEventSystem : MonoBehaviour
{
    private static PersistentEventSystem _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);   // İkinci kopyayı yok et
            return;
        }
        
       
    }
}

