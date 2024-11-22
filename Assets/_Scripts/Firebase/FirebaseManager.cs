using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;

public class FirebaseManager : MonoBehaviour
{
    private FirebaseApp app;
    public bool isFirebaseReady;

    [Header("Auth")] 
    //private Firebase.Auth auth;
    
    FirebaseFirestore rootDb;

    [Header("Collection_Account")] 
    private readonly string Collection_Account = "Accounts";
    
    [Header("Collection_DisplayName")] 
    private readonly string Collection_DisplayName = "DisplayName";
    
    [Header("Collection_Restrictions")] 
    private readonly string Collection_Restrictions = "Restrictions";

    [Header("Registration")]
    private bool checkingIfDisplayNameExists;
    private bool displayNameExists;
    private bool checkingIfEmailExists;
    private bool emailExists;
    
    [Header("Login")]
    private bool isLoginRunning;
    private AccountCollectionData loginResponse;
    
    #region Singleton

    public static FirebaseManager Instance;

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
        isFirebaseReady = false;
        
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.{
                isFirebaseReady = true;

                SetUpFirebase();
            } else {
                UnityEngine.Debug.LogError(System.String.Format(
                    "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.{
                isFirebaseReady = false;
            }
        });
    }

    private void SetUpFirebase()
    {
        if (!isFirebaseReady)
        {
            return;
        }
        
        rootDb = FirebaseFirestore.DefaultInstance;
    }
    
    #region Register / Login
    
    internal (bool, string) CreateNewUser(string email, string password)
    {
        bool isSuccessfull = false;
        string uid = "";
        
        var auth = FirebaseAuth.DefaultInstance;
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                isSuccessfull = false;
                uid = "";
                return;
            }
            if (task.IsFaulted) {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                isSuccessfull = false;
                uid = "";
                return;
            }

            // Firebase user has been created.
            AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);
            
            isSuccessfull = true;
            uid = result.User.UserId;
        });

        return (isSuccessfull, uid);
    }

    internal void CreateNewUserInFireStore(AccountCollectionData accountCollectionData, 
        DisplayNameCollectionData displayNameCollectionData,
        RestrictionCollectionData restrictionCollectionData)
    {
        StartCoroutine(CreateNewUserInFireStoreCor(accountCollectionData, displayNameCollectionData, restrictionCollectionData));
    }
    
    IEnumerator CreateNewUserInFireStoreCor(AccountCollectionData accountCollectionData, 
        DisplayNameCollectionData displayNameCollectionData,
        RestrictionCollectionData restrictionCollectionData)
    {
        CheckIfDisplayNameExists(accountCollectionData.Display_Name);

        while (checkingIfDisplayNameExists)
        {
            yield return null;
        }

        if (displayNameExists)
        {
            ToastManager.Instance.ShowToast("Display name already exist");
            Debug.Log("Display name already exist");
            yield break;
        }
        
        CheckIfEmailExists(accountCollectionData.Email);
        
        while (checkingIfEmailExists)
        {
            yield return null;
        }

        if (emailExists)
        {
            ToastManager.Instance.ShowToast("Email already exist");
            Debug.Log("Email already exist");
            yield break;
        }
        
        UpdateEntryToAccountCollection(accountCollectionData);
        UpdateEntryToDisplay_NameCollection(displayNameCollectionData);
        UpdateEntryToRestrictionCollection(restrictionCollectionData);
    }

    internal void LoginUser(string email, string password)
    {
        loginResponse = null;
        StartCoroutine(LoginUserCor(email, password));
    }

    IEnumerator LoginUserCor(String email, string password)
    {
        GetAllDataOfEmail(email);

        while (isLoginRunning)
        {
            yield return null;
        }
        
        DatabaseManager.LoginResponse?.Invoke(loginResponse, password);
    }
    
    #endregion

    #region Account Collection

    private void UpdateEntryToAccountCollection(AccountCollectionData accountCollectionData)
    {
        DocumentReference docRef = rootDb.Collection(Collection_Account).Document(accountCollectionData.Display_Name);
        
        docRef.SetAsync(accountCollectionData).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdateEntryToAccountCollection canceled");
                DatabaseManager.NewUserRegisterResponse?.Invoke(false);
                return;
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("UpdateEntryToAccountCollection encountered an error: " + task.Exception);
                DatabaseManager.NewUserRegisterResponse?.Invoke(false);
                return;
            }
            
            Debug.Log($"Added data in the {Collection_Account} collection with doc id {accountCollectionData.Display_Name}.");
            DatabaseManager.NewUserRegisterResponse?.Invoke(true);
        });
    }

    #endregion

    #region Display Name Collection

    private void UpdateEntryToDisplay_NameCollection(DisplayNameCollectionData displayNameCollectionData)
    {
        DocumentReference docRef = rootDb.Collection(Collection_DisplayName).Document(displayNameCollectionData.Display_Name);
        
        docRef.SetAsync(displayNameCollectionData).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdateEntryToDisplay_NameCollection canceled");
                return;
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("UpdateEntryToDisplay_NameCollection encountered an error: " + task.Exception);
                return;
            }
            
            Debug.Log($"Added data in the {Collection_DisplayName} collection with doc id {displayNameCollectionData.Display_Name}.");
        });
    }

    #endregion
    
    #region Restriction Collection

    private void UpdateEntryToRestrictionCollection(RestrictionCollectionData restrictionCollectionData)
    {
        DocumentReference docRef = rootDb.Collection(Collection_Restrictions).Document(restrictionCollectionData.Display_Name);
        
        docRef.SetAsync(restrictionCollectionData).ContinueWithOnMainThread(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("UpdateEntryToRestrictionCollection canceled");
                return;
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("UpdateEntryToRestrictionCollection encountered an error: " + task.Exception);
                return;
            }
            
            Debug.Log($"Added data in the {Collection_Restrictions} collection with doc id {restrictionCollectionData.Display_Name}.");
        });
    }

    #endregion
    
    #region Helper

    internal int GetServerTimestamp()
    {
        DateTime now = Timestamp.GetCurrentTimestamp().ToDateTime().ToUniversalTime();
        DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        int cur_time = (int)(now - epochStart).TotalSeconds;
        return cur_time;
    }

    private void CheckIfDisplayNameExists(string displayName)
    {
        DocumentReference docRef = rootDb.Collection(Collection_DisplayName).Document(displayName);

        checkingIfDisplayNameExists = true;
        
        docRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            var snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log(String.Format("Document data exists for {0} document", snapshot.Id));
            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
            
            checkingIfDisplayNameExists = false;
            displayNameExists = snapshot.Exists;
        });
    }
    
    //we can use uid and account collection once the firebase auth is fixed.
    private void CheckIfEmailExists(string email)
    {
        checkingIfEmailExists = true;
        emailExists = false;
        
        Query allQuery = rootDb.Collection(Collection_DisplayName);
        allQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot querySnapshot = task.Result;
            foreach (DocumentSnapshot snapshot in querySnapshot.Documents)
            {
                Debug.Log(String.Format("Document data found for {0} document", snapshot.Id));
                
                DisplayNameCollectionData displayNameCollectionData = snapshot.ConvertTo<DisplayNameCollectionData>();

                if (displayNameCollectionData.Email == email)
                {
                    emailExists = true;
                }
                // Newline to separate entries
                Debug.Log("");
            }
            
            checkingIfEmailExists = false;
        });
    }
    
    private void CheckIfEmailExists_ProperFun(string email, string displayName)
    {
        checkingIfEmailExists = true;
        emailExists = false;
        
        DocumentReference docRef = rootDb.Collection(Collection_Account).Document(displayName);

        checkingIfDisplayNameExists = true;
        
        docRef.GetSnapshotAsync().ContinueWith((task) =>
        {
            var snapshot = task.Result;
            if (snapshot.Exists)
            {
                AccountCollectionData accountCollectionData = snapshot.ConvertTo<AccountCollectionData>();

                if (accountCollectionData.Email == email)
                {
                    emailExists = true;
                }
                
                Debug.Log(String.Format("Document data exists for {0} document", snapshot.Id));
            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
            
            checkingIfEmailExists = false;
        });
    }

    private void GetAllDataOfEmail(string email)
    {
        isLoginRunning = true;
        
        Query allQuery = rootDb.Collection(Collection_Account);
        allQuery.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            QuerySnapshot querySnapshot = task.Result;
            foreach (DocumentSnapshot snapshot in querySnapshot.Documents)
            {
                Debug.Log(String.Format("Document data found for {0} document", snapshot.Id));
                
                AccountCollectionData response = snapshot.ConvertTo<AccountCollectionData>();

                if (response.Email == email)
                {
                    loginResponse = snapshot.ConvertTo<AccountCollectionData>();
                }
                // Newline to separate entries
                Debug.Log("");
            }
            
            isLoginRunning = false;
        });
    }
    
    #endregion
}
