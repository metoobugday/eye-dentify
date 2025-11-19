using UnityEngine;

public class EyeFollow : MonoBehaviour
{
    public RectTransform eyeArea;   // Gözün kendisi (örneðin LeftEye)
    public float moveRange = 20f;   // Piksel cinsinden sýnýr

    void Update()
    {
        // Fare pozisyonunu al (Canvas koordinatýna çevir)
        Vector2 mousePos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            eyeArea, Input.mousePosition, null, out mousePos);

        // Fare yönünü normalize et
        Vector2 direction = mousePos.normalized;

        // Sýnýrlandýr ve pozisyonu uygula
        transform.localPosition = direction * moveRange;
    }
}