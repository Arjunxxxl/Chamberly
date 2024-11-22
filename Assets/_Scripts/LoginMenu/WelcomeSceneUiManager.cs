using System;
using UnityEngine;

public class WelcomeSceneUiManager : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private Canvas welcomeCanvas;
    [SerializeField] private Canvas registerCanvas;
    [SerializeField] private Canvas loginCanvas; 
    
    [Header("SCripts")]
    [SerializeField] private WelcomeMenu welcomeMenu;
    [SerializeField] private RegisterMenu registerMenu;
    [SerializeField] private LoginMenu loginMenu;

    private bool animateMenu;
    
    #region Singleton

    public static WelcomeSceneUiManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    private void Start()
    {
        welcomeMenu.SetUp();
        registerMenu.SetUp();
        loginMenu.SetUp();
    }

    internal void EnableCanvas(WelcomeSceneStates welcomeSceneStates)
    {
        welcomeCanvas.enabled = welcomeSceneStates == WelcomeSceneStates.Welcome;
        registerCanvas.enabled = welcomeSceneStates == WelcomeSceneStates.Register;
        loginCanvas.enabled = welcomeSceneStates == WelcomeSceneStates.Login;
    }

    internal void SetMenuAnimation(bool animateMenu)
    {
        this.animateMenu = animateMenu;
    }
}
