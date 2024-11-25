using Firebase.Auth;
using Firebase.Database;
using Gamekit3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class UserSavingAndLoading : MonoBehaviour
{
    private string folderPath = Application.streamingAssetsPath;
    private string fileName = "PlayerSaveData.json";
    private string filePath;
    private FirebaseAuth authenticationInstance;
    private FirebaseUser userProfileData;
    private DatabaseReference databaseReference;
    private Coroutine interactingWithDatabaseRoutine;

    [SerializeField] private string userID;
    private PlayerSaveData localPlayerData;
    private PlayerSaveData databasePlayerData;

    [SerializeField] private AudioMixer audioMixer;

    [Header("Debugging")]
    [SerializeField] private bool simulateNoInternet;

    private void Awake()
    {
        filePath = Path.Combine(folderPath, fileName);
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
        localPlayerData = null;
        databasePlayerData = null;
    }

    private IEnumerator Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable || simulateNoInternet)
        {
            Debug.LogWarning("No internet");
            LoadPlayerData();
            yield break;
        }

        authenticationInstance = FirebaseAuth.DefaultInstance;

        if (authenticationInstance == null)
        {
            Debug.LogError("No authentication");
            LoadPlayerData();
            yield break;
        }

        userProfileData = authenticationInstance.CurrentUser;
        if (!string.IsNullOrEmpty(userID))
        {
            userID = userProfileData.UserId;
        }
        else
        {
            LoadPlayerData();
            Debug.Log("No user logged in");
            yield break;
        }

        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        LoadPlayerData();
        yield return null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K) && interactingWithDatabaseRoutine == null)
        {
            SavePlayerData();
        }
        if (Input.GetKeyDown(KeyCode.L) && interactingWithDatabaseRoutine == null)
        {
            LoadPlayerData();
        }
    }

    public void SavePlayerData()
    {
        GameObject player = FindObjectOfType<CharacterController>().gameObject; // Get the player character
        //InventoryController playerInventory = player.GetComponent<InventoryController>(); // Get the player inventory
        localPlayerData = new PlayerSaveData(); // The type of data to save
        localPlayerData.PlayerName = userProfileData.DisplayName;
        localPlayerData.PlayerHealth = player.GetComponent<Damageable>().currentHitPoints;
        localPlayerData.PlayerPosition = player.transform.position;
        audioMixer.GetFloat("MasterVolume", out float masterValue);
        localPlayerData.VolumeMaster = masterValue;
        audioMixer.GetFloat("SFXVolume", out float sfxValue);
        localPlayerData.VolumeSFX = sfxValue;
        localPlayerData.UTCDateTimeModified = DateTime.UtcNow.ToString();
        string saveDataString = JsonUtility.ToJson(localPlayerData, true); // Convert to json string
        File.WriteAllText(filePath, saveDataString); // Save to file
        Debug.Log($"{userProfileData.DisplayName} game data saved locally");
        interactingWithDatabaseRoutine = StartCoroutine(SavePlayerDataToServer());
    }

    private IEnumerator SavePlayerDataToServer()
    {
        // Check for internet connection
        if (Application.internetReachability == NetworkReachability.NotReachable || simulateNoInternet)
        {
            Debug.LogError("No internet, can't save to server");
            interactingWithDatabaseRoutine = null;
            yield break;
        }

        string jsonData = JsonUtility.ToJson(localPlayerData, true);
        Task sendJson = databaseReference.Child("users").Child(userID).SetRawJsonValueAsync(jsonData);

        while (!sendJson.IsCompleted && !(sendJson.IsFaulted || sendJson.IsCanceled))
        {
            yield return null;
        }

        if (sendJson.IsFaulted)
        {
            Debug.LogError("Error with saving player game data to server");
            interactingWithDatabaseRoutine = null;
            yield break;
        }

        Debug.Log($"{userProfileData.DisplayName} game data saved to server");
        interactingWithDatabaseRoutine = null;
        yield return null;
    }

    public void LoadPlayerData()
    {
        interactingWithDatabaseRoutine = StartCoroutine(LoadPlayerDataFromServer());
    }

    private IEnumerator LoadPlayerDataFromServer()
    {
        // First load in local save data
        if (File.Exists(filePath))
        {
            string playerDataString = File.ReadAllText(filePath);
            localPlayerData = JsonUtility.FromJson<PlayerSaveData>(playerDataString);
        }
        else
        {
            Debug.Log("No local save data to load");
        }

        // Check for internet connection
        if (Application.internetReachability == NetworkReachability.NotReachable || simulateNoInternet || string.IsNullOrEmpty(userID))
        {
            Debug.LogWarning("No internet or user logged in");
            LoadLocalPlayerData();
            interactingWithDatabaseRoutine = null;
            yield break;
        }

        Task<DataSnapshot> userData = databaseReference.Child("users").Child(userID).GetValueAsync();

        while (!userData.IsCompleted)
        {
            yield return null;
        }

        if (userData.IsFaulted || userData.IsCanceled)
        {
            Debug.LogError("Error with loading server data");
            LoadLocalPlayerData();
            interactingWithDatabaseRoutine = null;
            yield break;
        }

        DataSnapshot snapShotRetrieved = userData.Result;
        string returnedJson = snapShotRetrieved.GetRawJsonValue();

        if (string.IsNullOrEmpty(returnedJson))
        {
            Debug.LogWarning("Server player data was null or empty");
            LoadLocalPlayerData();
            interactingWithDatabaseRoutine = null;
            yield break;
        }

        databasePlayerData = JsonUtility.FromJson<PlayerSaveData>(returnedJson);
        Debug.Log("Player data loaded from server");

        // Check if local data is equal or more recent than server data (File.Exists used if file is deleted while player data is loaded in game)
        if (File.Exists(filePath) && localPlayerData != null && DateTime.Parse(databasePlayerData.UTCDateTimeModified) <= DateTime.Parse(localPlayerData.UTCDateTimeModified))
        {
            Debug.Log("Loading local data as it is either more recent or the same");
            LoadLocalPlayerData();
            // Save data to server only if local is more recent
            if (DateTime.Parse(databasePlayerData.UTCDateTimeModified) < DateTime.Parse(localPlayerData.UTCDateTimeModified))
            {
                interactingWithDatabaseRoutine = StartCoroutine(SavePlayerDataToServer()); // Save the local data to the server
                Debug.Log("Saved local data to server as it was more recent");
                yield break;
            }
        }
        else
        {
            // Set local data with server data
            localPlayerData = databasePlayerData;
            LoadLocalPlayerData();

            // Save locally
            string saveDataString = JsonUtility.ToJson(localPlayerData, true);
            File.WriteAllText(filePath, saveDataString);
        }

        interactingWithDatabaseRoutine = null;
        yield return null;
    }

    private void LoadLocalPlayerData()
    {
        if (localPlayerData != null)
        {
            Debug.Log("Loading from local data instead");
            GameObject player = FindObjectOfType<CharacterController>().gameObject; // Get the player character
            player.GetComponent<Damageable>().SetHealth(localPlayerData.PlayerHealth);
            player.transform.position = localPlayerData.PlayerPosition;
            FindObjectOfType<PlayerNamePlate>().GetComponentInChildren<TextMeshProUGUI>().text = localPlayerData.PlayerName;
            audioMixer.SetFloat("MasterVolume", localPlayerData.VolumeMaster);
            audioMixer.SetFloat("SFXVolume", localPlayerData.VolumeSFX);
        }
        else
        {
            Debug.Log("No data loaded as there was no local save file");
        }
    }
}