using System.Collections;
using Firebase.Firestore;

[System.Serializable]
public class UserData
{
    public string uid;
    public long AITimestamp;
    public int Coins;
    public long DailyCoinsTimestamp;
    public string Display_Name;
    public string Email;
    public string Password;
    public int Age;
    public string avatarName;
    public string firstGender;
    public string[] interests;
    public bool isModerator;
    public long timestamp;
    public Hashtable privacy;
    public string userIntentions;
    public string selectedRole;
    public string accountCreatedAt;
    public string platform;
}

[FirestoreData]
public class AccountCollectionData
{
    [FirestoreProperty]
    public string uid{ get; set; }
    [FirestoreProperty]
    public long AITimestamp{ get; set; }
    [FirestoreProperty]
    public int Coins{ get; set; }
    [FirestoreProperty]
    public long DailyCoinsTimestamp{ get; set; }
    [FirestoreProperty]
    public string Display_Name{ get; set; }
    [FirestoreProperty]
    public string Email{ get; set; }
    [FirestoreProperty]
    public string Password{ get; set; }
    [FirestoreProperty]
    public int Age{ get; set; }
    [FirestoreProperty]
    public string avatarName{ get; set; }
    [FirestoreProperty]
    public string firstGender{ get; set; }
    [FirestoreProperty]
    public string[] interests{ get; set; }
    [FirestoreProperty]
    public bool isModerator{ get; set; }
    [FirestoreProperty]
    public long timestamp{ get; set; }
    [FirestoreProperty]
    public Hashtable privacy{ get; set; }
    [FirestoreProperty]
    public string userIntentions{ get; set; }
    [FirestoreProperty]
    public string selectedRole{ get; set; }
    [FirestoreProperty]
    public string accountCreatedAt{ get; set; }
    [FirestoreProperty]
    public string platform{ get; set; }
}

[FirestoreData]
public class DisplayNameCollectionData
{
    [FirestoreProperty]
    public string uid{ get; set; }
    [FirestoreProperty]
    public string Display_Name{ get; set; }
    [FirestoreProperty]
    public string Email{ get; set; }
}

[FirestoreData]
public class RestrictionCollectionData
{
    [FirestoreProperty]
    public string Display_Name{ get; set; }
    [FirestoreProperty]
    public int RestrictedUntil{ get; set; }
}