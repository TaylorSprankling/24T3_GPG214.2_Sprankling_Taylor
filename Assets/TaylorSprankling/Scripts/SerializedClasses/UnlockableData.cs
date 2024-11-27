using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class UnlockableData
{
    [SerializeField] private string unlockableFolder;
    [SerializeField] private string unlockableID;
    public int CurrentActionsRecorded = 0;
    public bool IsUnlocked = false;
    public bool HasPreviouslyBeenDisplayed = false;

    public string UnlockableID { get => unlockableID; set => unlockableID = value; }

    public void Unlock()
    {
        IsUnlocked = true;
    }

    public void UpdateHasBeenDisplayed()
    {
        HasPreviouslyBeenDisplayed = true;
    }

    // Following Nathan's videos I placed this here, but I would probably prefer this logic in the UnlockableItem scriptable object
    // as it makes more sense to keep this as a data file but also I'm saving the unlockable folder and ID to the server twice essentially.
    // Depending on time constraints I may move this over
    public IEnumerator SaveData(DatabaseReference databaseReference, string userId)
    {
        // Check for no internet connection or no user logged in
        if (Application.internetReachability == NetworkReachability.NotReachable || (FirebaseAuth.DefaultInstance.CurrentUser != null && string.IsNullOrEmpty(FirebaseAuth.DefaultInstance.CurrentUser.UserId)))
        {
            Debug.LogError("No internet, can't save to server");
            yield break;
        }

        string jsonData = JsonUtility.ToJson(this, true);
        Task sendJson = databaseReference.Child("users").Child(userId).Child(unlockableFolder).Child(UnlockableID).SetRawJsonValueAsync(jsonData);

        while (!sendJson.IsCompleted && !(sendJson.IsFaulted || sendJson.IsCanceled))
        {
            yield return null;
        }

        // Check if saving to server failed
        if (sendJson.IsFaulted)
        {
            Debug.LogError("Error with saving player game data to server");
            yield break;
        }

        Debug.Log($"{FirebaseAuth.DefaultInstance.CurrentUser.DisplayName} game data saved to server");
        yield return null;
    }

    public IEnumerator LoadData(DatabaseReference databaseReference, string userId)
    {
        // Check for no internet connection or no user logged in
        if (Application.internetReachability == NetworkReachability.NotReachable || (FirebaseAuth.DefaultInstance.CurrentUser != null && string.IsNullOrEmpty(FirebaseAuth.DefaultInstance.CurrentUser.UserId)))
        {
            Debug.LogError("No internet, can't load from server");
            yield break;
        }

        Task<DataSnapshot> userData = databaseReference.Child("users").Child(userId).Child(unlockableFolder).Child(UnlockableID).GetValueAsync();

        while (!userData.IsCompleted)
        {
            yield return null;
        }

        // Check if data wasn't loaded
        if (userData.IsFaulted || userData.IsCanceled)
        {
            Debug.LogError("Error with loading server data");
            yield break;
        }

        DataSnapshot snapShotRetrieved = userData.Result;
        string returnedJson = snapShotRetrieved.GetRawJsonValue();

        // Check if retrieved Json is null or empty
        if (!string.IsNullOrEmpty(returnedJson))
        {
            UnlockableData data = JsonUtility.FromJson<UnlockableData>(returnedJson);
            unlockableFolder = data.unlockableFolder;
            UnlockableID = data.UnlockableID;
            CurrentActionsRecorded = data.CurrentActionsRecorded;
            IsUnlocked = data.IsUnlocked;
            HasPreviouslyBeenDisplayed = data.HasPreviouslyBeenDisplayed;
        }
        else
        {
            Debug.LogWarning("Server player unlock data was null or empty");
            yield break;
        }

        yield return null;
    }
}