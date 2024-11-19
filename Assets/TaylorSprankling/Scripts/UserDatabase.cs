using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserDatabase : MonoBehaviour
{
    public PlayerSaveData myCurrentSaveData;
    public PlayerSaveData saveDataFromServer;

    private FirebaseAuth authenticationInstance;
    private FirebaseUser userData;
    private DatabaseReference databaseReference;
    private Coroutine interactingWithDatabaseRoutine;

    public string userID;

    [Header("Debugging")]
    [SerializeField] private bool useDefaultCredentials;
    private string defaultEmail = "mytest@testmail.com";
    private string defaultPassword = "password";
    [SerializeField] private bool simulateNoInternet;


    private IEnumerator Start()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable || simulateNoInternet)
        {
            Debug.LogError("No internet");
            yield break;
        }

        InitializeFirebase();

        if (authenticationInstance == null)
        {
            Debug.LogError("No authentication");
            yield break;
        }

        //if (useDefaultCredentials)
        //{
        //    if (LogInOrCreateUserRoutine == null)
        //    {
        //        LogInOrCreateUserRoutine = StartCoroutine(SignInUser(defaultEmail, defaultPassword));
        //        Debug.Log("Logging in default user");
        //    }

        //    while (LogInOrCreateUserRoutine != null)
        //    {
        //        yield return null;
        //    }
        //}
    }

    private void InitializeFirebase()
    {
        authenticationInstance = FirebaseAuth.DefaultInstance;
    }

    private void InitAndGetDatabaseReference()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private IEnumerator SavePlayerData()
    {
        yield return null;
    }

    private IEnumerator LoadPlayerData()
    {
        yield return null;
    }
}