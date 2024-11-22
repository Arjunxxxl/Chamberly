using UnityEngine;
using TMPro;

public class ToastManager : MonoBehaviour
{
    private Canvas canvas;
    public TMP_Text toastTxt;
    public float toastDuration;
    private float toastTimeElapsed;
    public bool isToastShowing;
    
    #region Singleton

    public static ToastManager Instance;

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
        canvas = GetComponent<Canvas>();
        canvas.enabled = false;
        isToastShowing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isToastShowing)
        {
            toastTimeElapsed += Time.deltaTime;

            if (toastTimeElapsed >= toastDuration)
            {
                toastTimeElapsed = 0;
                isToastShowing = false;
                canvas.enabled = false;
            }
        }
    }

    internal void ShowToast(string toastMessage)
    {
        isToastShowing = true;
        toastTimeElapsed = 0;

        toastTxt.text = toastMessage;
        canvas.enabled = true;
    }
}
