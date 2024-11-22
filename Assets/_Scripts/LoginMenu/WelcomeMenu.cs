using UnityEngine;
using UnityEngine.UI;

public class WelcomeMenu : MonoBehaviour
{
    [Header("Buttons")] 
    [SerializeField] private Button registerButton;
    [SerializeField] private Button loginButton;
    
    internal void SetUp()
    {
        registerButton.onClick.AddListener(OnClickRegisterButton);
        loginButton.onClick.AddListener(OnClickLoginButton);
    }

    private void OnClickRegisterButton()
    {
        WelcomeSceneUiManager.Instance.SetMenuAnimation(true);
        WelcomeSceneManager.Instance.SetState(WelcomeSceneStates.Register);
    }

    private void OnClickLoginButton()
    {
        WelcomeSceneManager.Instance.SetState(WelcomeSceneStates.Login);
    }
}
