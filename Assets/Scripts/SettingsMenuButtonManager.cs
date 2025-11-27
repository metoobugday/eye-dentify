using UnityEngine;

public class SettingsMenuButtonManager : MonoBehaviour
{
    [SerializeField] MainMenuManager.SettingsButtons _buttonType;

    public void ButtonClicked()
    {
        MainMenuManager.Instance.SettingsButtonClicked(_buttonType);
    }
}
