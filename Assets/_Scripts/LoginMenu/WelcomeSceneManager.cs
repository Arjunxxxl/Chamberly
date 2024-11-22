using UnityEngine;

public class WelcomeSceneManager : MonoBehaviour
{
    [Header("States")] 
    [SerializeField] private WelcomeSceneStates welcomeSceneState = WelcomeSceneStates.Unknown;
    
    #region Singleton

    public static WelcomeSceneManager Instance;

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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetState(WelcomeSceneStates.Welcome);
    }

    #region State

    internal void SetState(WelcomeSceneStates welcomeSceneStates)
    {
        if (this.welcomeSceneState == welcomeSceneStates)
        {
            return;
        }

        this.welcomeSceneState = welcomeSceneStates;

        WelcomeSceneUiManager.Instance.EnableCanvas(this.welcomeSceneState);
    }

    internal WelcomeSceneStates GetState()
    {
        return welcomeSceneState;
    }

    #endregion
}
