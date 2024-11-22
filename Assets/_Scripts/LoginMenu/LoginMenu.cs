using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginMenu : MonoBehaviour
{
    [Header("Buttons")] 
    [SerializeField] private Button loginButton;
    [SerializeField] private Button backButton;

    [Header("Input")] 
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    
    internal void SetUp()
    {
        loginButton.onClick.AddListener(OnClickLoginButton);
        backButton.onClick.AddListener(OnClickBackButton);
    }

    private void OnClickLoginButton()
    {
        string email = emailInputField.text;
        string pass = passwordInputField.text;
        
        DatabaseManager.Instance.LoginUser(email, pass);
    }

    private void OnClickBackButton()
    {
        WelcomeSceneManager.Instance.SetState(WelcomeSceneStates.Welcome);
    }
}
