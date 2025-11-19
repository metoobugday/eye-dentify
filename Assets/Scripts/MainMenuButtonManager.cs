using UnityEngine;

public class MainMenuButtonManager : MonoBehaviour
{
    [SerializeField] MainMenuManager.MainMenuButtons _buttonType;
    public void ButtonClicked()
    {
        MainMenuManager.Instance.MainMenuButtonClicked(_buttonType);
    }
}
