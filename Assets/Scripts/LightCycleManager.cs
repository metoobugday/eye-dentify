using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(-10)]
public class LightCycleManager : MonoBehaviour
{
    [Header("Süreler (saniye)")]
    public float lightsOnDuration = 1.5f;
    public float lightsOffDuration = 5f;

    [Header("Baþlangýç")]
    public bool startWithLightsOff = true;

    [Header("Olaylar")]
    public UnityEvent OnLightsOn;
    public UnityEvent OnLightsOff;

    float timer;
    bool lightsOn;
    public bool LightsOn => lightsOn;
    public float TimeLeft => timer;

    void Start()
    {
        lightsOn = !startWithLightsOff; // ilk Update'te toggle olsun diye
        timer = 0f;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Toggle();
        }
    }

    void Toggle()
    {
        lightsOn = !lightsOn;
        timer = lightsOn ? lightsOnDuration : lightsOffDuration;

        if (lightsOn) OnLightsOn?.Invoke();
        else OnLightsOff?.Invoke();
    }
}

