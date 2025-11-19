using UnityEngine;

public class CreditsMenuButtonManager : MonoBehaviour
{
    [SerializeField] MainMenuManager.CreditsButtons _buttonType;

    public void ButtonClicked()
    {
        MainMenuManager.Instance.CreditsButtonClicked(_buttonType);
    }
}
