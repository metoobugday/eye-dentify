using UnityEngine;
using UnityEngine.Rendering.Universal; // Light2D

public class LightLinker : MonoBehaviour
{
    [Header("Referanslar")]
    public LightCycleManager cycle; // sahnedeki LightCycleManager
    public Light2D globalLight;     // sahnedeki Global Light 2D
    public Light2D playerLight;     // Player altýndaki Point Light 2D

    [Header("Kapalý Durum (keþif)")]
    public float globalOffIntensity = 0.1f;
    public float playerOffOuter = 4.0f;

    [Header("Açýk Durum (tam aydýnlýk)")]
    public float globalOnIntensity = 1.0f;
    public float playerOnOuter = 0.1f; // neredeyse yok

    [Header("Yumuþatma")]
    public float smooth = 6f; // 0 = anlýk, 6 = hýzlý yumuþak

    void Reset()
    {
        // Sahneye eklendiðinde otomatik bulmayý dener
        if (!cycle) cycle = Object.FindFirstObjectByType<LightCycleManager>();
        if (!globalLight) globalLight = Object.FindFirstObjectByType<Light2D>();
    }

    void Update()
    {
        if (!cycle) return;

        // hedef deðerleri seç
        float targetGlobal = cycle.LightsOn ? globalOnIntensity : globalOffIntensity;
        float targetRadius = cycle.LightsOn ? playerOnOuter : playerOffOuter;

        // yumuþatma (lerp)
        if (globalLight)
            globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetGlobal, smooth * Time.deltaTime);

        if (playerLight)
            playerLight.pointLightOuterRadius = Mathf.Lerp(playerLight.pointLightOuterRadius, targetRadius, smooth * Time.deltaTime);
    }
}