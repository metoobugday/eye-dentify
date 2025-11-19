using UnityEngine;

public class LevelState : MonoBehaviour
{
    [Header("Collectibles")]
    public int requiredItemCount = 3;   // Bu levelde kaç item gerekli
    public int collectedItemCount = 0;  // Kaçını topladık (sadece okunur gibi düşün)

    // İtem toplandığında bu metod çağrılacak
    public void AddItem()
    {
        collectedItemCount++;
        Debug.Log($"Item collected: {collectedItemCount}/{requiredItemCount}");
        // Kapı/level geçiş kontrolünü sonraki adımda ekleyeceğiz.
    }
}

