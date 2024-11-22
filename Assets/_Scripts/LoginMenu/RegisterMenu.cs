using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterMenu : MonoBehaviour
{
    private Canvas canvas;
    private RectTransform canvasRectT;
    
    [Header("Buttons")] 
    [SerializeField] private Button registerButton;
    [SerializeField] private Button backButton;

    [Header("Input")] 
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private TMP_InputField emailInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField repasswordInputField;

    [Header("Positions")] 
    [SerializeField] private Vector2 inScreenOffset;
    [SerializeField] private Vector2 outScreenOffset;

    [Header("Animate")] 
    [SerializeField] private bool animate;
    [SerializeField] private bool isShowing;
    [SerializeField] private float animationSpeed;

    private void Update()
    {
        AnimateMenu();
    }

    internal void SetUp()
    {
        canvas = GetComponent<Canvas>();
        canvasRectT = canvas.GetComponent<RectTransform>();
        
        registerButton.onClick.AddListener(OnClickRegisterButton);
        backButton.onClick.AddListener(OnClickBackButton);

        int screenWidth = Screen.width;
        
        inScreenOffset = new Vector2(0, 0);
        outScreenOffset = new Vector2(screenWidth, screenWidth * -1);
    }

    #region Show/Hide Animation

    internal void ShowMenu(bool show, bool isInstant)
    {
        if (isInstant)
        {
            canvasRectT.offsetMin = new Vector2(show ? inScreenOffset.x : outScreenOffset.x, canvasRectT.offsetMin.y);
            canvasRectT.offsetMax = new Vector2(show ? inScreenOffset.y : outScreenOffset.y, canvasRectT.offsetMax.y);
            return;
        }

        animate = true;
        isShowing = show;
    }

    private void AnimateMenu()
    {
        if (!animate)
        {
            return;
        }

        Vector2 finalMin = new Vector2(isShowing ? inScreenOffset.x : outScreenOffset.x, canvasRectT.offsetMin.y);
        Vector2 finalMax = new Vector2(isShowing ? inScreenOffset.y : outScreenOffset.y, canvasRectT.offsetMax.y);
        
        canvasRectT.offsetMin = Vector3.Lerp(canvasRectT.offsetMin, finalMin, 1 - Mathf.Pow(0.5f, Time.deltaTime * animationSpeed));
        canvasRectT.offsetMax = Vector3.Lerp(canvasRectT.offsetMax, finalMax, 1 - Mathf.Pow(0.5f, Time.deltaTime * animationSpeed));

        if (Mathf.Abs(canvasRectT.offsetMin.x - finalMin.x) < 0.05f)
        {
            animate = false;
            
            canvasRectT.offsetMin = finalMin;
            canvasRectT.offsetMax = finalMax;
        }
    }

    #endregion

    #region Buttons

    private void OnClickRegisterButton()
    {
        string email = emailInputField.text;
        string displayName = nameInputField.text;
        string pass = passwordInputField.text;
        string repass = repasswordInputField.text;
        
        if (displayName == "")
        {
            ToastManager.Instance.ShowToast("Enter Display Name");
            Debug.Log("Enter Display Name");
            return;
        }
        
        if (email == "")
        {
            ToastManager.Instance.ShowToast("Enter Email");
            Debug.Log("Enter Email");
            return;
        }
        
        if (pass == "" || repass == "")
        {
            ToastManager.Instance.ShowToast("Enter password");
            Debug.Log("Enter password");
            return;
        }

        if (!CheckIfPasswordIsOfProperLength(pass))
        {
            ToastManager.Instance.ShowToast("Password must be 8 letters long");
            Debug.Log("Password must be 8 letters long");
            return;
        }
        
        if (pass != repass)
        {
            ToastManager.Instance.ShowToast("Password does not match");
            Debug.Log("Password does not match");
            return;
        }
        
        DatabaseManager.Instance.RegisterNewUser(displayName, email, pass);
    }

    private void OnClickBackButton()
    {
        WelcomeSceneManager.Instance.SetState(WelcomeSceneStates.Welcome);
    }

    #endregion

    #region Helper

    private bool CheckIfPasswordIsOfProperLength(string password)
    {
        return password.Length >= 8;
    }

    #endregion
}
