using System;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    [Header("User Data")] 
    [SerializeField] private UserData userData;
    private AccountCollectionData accountCollectionData;
    private DisplayNameCollectionData displayNameCollectionData;
    private RestrictionCollectionData restrictionCollectionData;

    public static Action<bool> NewUserRegisterResponse;
    public static Action<AccountCollectionData, string> LoginResponse;
    
    #region Singleton

    public static DatabaseManager Instance;

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

    private void OnEnable()
    {
        NewUserRegisterResponse += OnNewUserRegistered;
        LoginResponse += OnLoginResponse;
    }

    private void OnDisable()
    {
        NewUserRegisterResponse -= OnNewUserRegistered;
        LoginResponse -= OnLoginResponse;
    }

    #region Register
    
    internal void RegisterNewUser(string displayName, string email, string password)
    {
        int serverTimeStamp = FirebaseManager.Instance.GetServerTimestamp();
        
        userData = new UserData();
        userData.Display_Name = displayName;
        userData.Email = email;
        userData.Password = password;
        userData.AITimestamp = serverTimeStamp;
        userData.timestamp = serverTimeStamp;
        
        accountCollectionData = new AccountCollectionData();
        accountCollectionData.Display_Name = displayName;
        accountCollectionData.Email = email;
        accountCollectionData.Password = password;
        
        displayNameCollectionData = new DisplayNameCollectionData();
        displayNameCollectionData.Display_Name = displayName;
        displayNameCollectionData.Email = email;
        displayNameCollectionData.uid = "";
        
        restrictionCollectionData = new RestrictionCollectionData();
        restrictionCollectionData.Display_Name = displayName;
        restrictionCollectionData.RestrictedUntil = serverTimeStamp;

        /*var res = FirebaseManager.Instance.CreateNewUser(email, password);

        if (res.Item1)
        {
            userData.uid = res.Item2;
            Debug.Log("New user registered");
        }
        else
        {
            Debug.Log("Fail to register new user");
        }*/

        FirebaseManager.Instance.CreateNewUserInFireStore(accountCollectionData, displayNameCollectionData, restrictionCollectionData);
    }

    private void OnNewUserRegistered(bool isSuccessful)
    {
        if (!isSuccessful)
        {
            ToastManager.Instance.ShowToast("Registration Failed");
            Debug.Log("Registration Failed");
            return;
        }
        
        ToastManager.Instance.ShowToast("Registration Successful - Entering Game");
        Debug.Log("Registration Successful - Entering Game");
    }
    
    #endregion

    #region Login

    internal void LoginUser(string email, string password)
    {
        FirebaseManager.Instance.LoginUser(email, password);
    }
    
    private void OnLoginResponse(AccountCollectionData accountCollectionData, string enteredPassword)
    {
        if (accountCollectionData == null)
        {
            ToastManager.Instance.ShowToast("Email does not exists");
            Debug.Log("Email does not exists");
            return;
        }
        
        if (accountCollectionData.Password != enteredPassword)
        {
            ToastManager.Instance.ShowToast("Wrong password");
            Debug.Log("Wrong password");
            return;
        }

        userData = new UserData();
        userData.uid = accountCollectionData.uid;
        userData.AITimestamp = accountCollectionData.AITimestamp;
        userData.Coins = accountCollectionData.Coins;
        userData.DailyCoinsTimestamp = accountCollectionData.DailyCoinsTimestamp;
        userData.Display_Name = accountCollectionData.Display_Name;
        userData.Email = accountCollectionData.Email;
        userData.Password = accountCollectionData.Password;
        userData.Age = accountCollectionData.Age;
        userData.avatarName = accountCollectionData.avatarName;
        userData.firstGender = accountCollectionData.firstGender;
        userData.interests = accountCollectionData.interests;
        userData.isModerator = accountCollectionData.isModerator;
        userData.timestamp = accountCollectionData.timestamp;
        userData.privacy = accountCollectionData.privacy;
        userData.userIntentions = accountCollectionData.userIntentions;
        userData.selectedRole = accountCollectionData.selectedRole;
        userData.accountCreatedAt = accountCollectionData.accountCreatedAt;
        userData.platform = accountCollectionData.platform;
        
        ToastManager.Instance.ShowToast("Entering Game");
        Debug.Log("Entering Game");
    }

    #endregion
}
