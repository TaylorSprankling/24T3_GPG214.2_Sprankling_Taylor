using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserAuthentication : MonoBehaviour
{
    [Header("Main Menu Items")]
    [SerializeField] Transform mainMenuScreenPanel;
    [SerializeField] private Button existingUserButton;
    [SerializeField] private Button newUserButton;

    [Header("Login Screen Items")]
    [SerializeField] Transform loginScreenPanel;
    [SerializeField] private TMP_InputField userDisplayName;
    [SerializeField] private TMP_InputField userEmail;
    [SerializeField] private TMP_InputField userPassword;
    [SerializeField] private Button logInButton;
    [SerializeField] private Button signOutButton;
    [SerializeField] private Button backButton;

    [Header("Debugging")]
    [SerializeField] private bool useDefaultCredentials;
    private string defaultEmail = "mytest@testmail.com";
    private string defaultPassword = "password";
    [SerializeField] private bool simulateNoInternet;

    [HideInInspector] public static bool isUserAuthenticated;

    private FirebaseAuth authenticationInstance;
    private FirebaseUser userProfileData;

    private Coroutine LogInOrCreateUserRoutine;

    private IEnumerator Start()
    {
        SetupMainMenu();

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

        if (useDefaultCredentials)
        {
            if (LogInOrCreateUserRoutine == null)
            {
                LogInOrCreateUserRoutine = StartCoroutine(SignInUser(defaultEmail, defaultPassword));
                Debug.Log("Logging in default user");
            }

            while (LogInOrCreateUserRoutine != null)
            {
                yield return null;
            }

            mainMenuScreenPanel.gameObject.SetActive(false);
            loginScreenPanel.gameObject.SetActive(false);
        }
    }

    private void SetupMainMenu()
    {
        existingUserButton.onClick.RemoveAllListeners();
        existingUserButton.onClick.AddListener(ExistingUser);
        newUserButton.onClick.RemoveAllListeners();
        newUserButton.onClick.AddListener(NewUser);
        signOutButton.onClick.RemoveAllListeners();
        signOutButton.onClick.AddListener(SignOut);
        mainMenuScreenPanel.gameObject.SetActive(true);
        loginScreenPanel.gameObject.SetActive(false);
    }

    private void NewUser()
    {
        mainMenuScreenPanel.gameObject.SetActive(false);
        loginScreenPanel.gameObject.SetActive(true);
        userDisplayName.gameObject.SetActive(true);
        if (!isUserAuthenticated)
        {
            signOutButton.interactable = false;
        }
        logInButton.GetComponentInChildren<TMP_Text>().text = "Sign Up";
        logInButton.onClick.RemoveAllListeners();
        logInButton.onClick.AddListener(CreateUser);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(SetupMainMenu);
    }

    private void ExistingUser()
    {
        mainMenuScreenPanel.gameObject.SetActive(false);
        loginScreenPanel.gameObject.SetActive(true);
        userDisplayName.gameObject.SetActive(false);
        if (!isUserAuthenticated)
        {
            signOutButton.interactable = false;
        }
        logInButton.GetComponentInChildren<TMP_Text>().text = "Log In";
        logInButton.onClick.RemoveAllListeners();
        logInButton.onClick.AddListener(SignInUser);
        backButton.onClick.RemoveAllListeners();
        backButton.onClick.AddListener(SetupMainMenu);
    }

    private void InitializeFirebase()
    {
        authenticationInstance = FirebaseAuth.DefaultInstance;
    }

    private void CreateUser()
    {
        if (LogInOrCreateUserRoutine == null)
        {
            LogInOrCreateUserRoutine = StartCoroutine(CreateNewUser(userEmail.text.Trim(), userPassword.text.Trim(), userDisplayName.text.Trim()));
        }
    }

    private void SignInUser()
    {
        if (LogInOrCreateUserRoutine == null)
        {
            LogInOrCreateUserRoutine = StartCoroutine(SignInUser(userEmail.text.Trim(), userPassword.text.Trim()));
        }
    }

    private IEnumerator CreateNewUser(string email, string password, string displayName)
    {
        Task<AuthResult> creatingUserTask = authenticationInstance.CreateUserWithEmailAndPasswordAsync(email, password);

        while (!creatingUserTask.IsCompleted)
        {
            yield return null;
        }

        if (creatingUserTask.IsCompletedSuccessfully)
        {
            Debug.Log("New user profile created");
            userProfileData = creatingUserTask.Result.User;
            UserProfile newProfile = new UserProfile();
            newProfile.DisplayName = displayName;
            Task updateProfile = userProfileData.UpdateUserProfileAsync(newProfile);

            while (!updateProfile.IsCompleted)
            {
                yield return null;
            }

            if (updateProfile.IsCompletedSuccessfully)
            {
                signOutButton.interactable = true;
                logInButton.interactable = false;
                isUserAuthenticated = true;
                Debug.Log("New user profile data updated");
            }
            else
            {
                Debug.Log("User profile was not created");
            }
        }
        else
        {
            Debug.Log($"New user creation failed {creatingUserTask.Exception}");
        }
        LogInOrCreateUserRoutine = null;
        yield return null;
    }

    private IEnumerator SignInUser(string email, string password)
    {
        Task<AuthResult> signInUserTask = authenticationInstance.SignInWithEmailAndPasswordAsync(email, password);

        while (!signInUserTask.IsCompleted)
        {
            yield return null;
        }

        if (signInUserTask.IsCompletedSuccessfully)
        {
            userProfileData = signInUserTask.Result.User;
            signOutButton.interactable = true;
            logInButton.interactable = false;
            isUserAuthenticated = true;
            Debug.Log($"User '{userProfileData.DisplayName}' logged in successfully");
        }
        else
        {
            Debug.Log($"Failed to sign in user {signInUserTask.Exception}");
        }
        LogInOrCreateUserRoutine = null;
        yield return null;
    }

    private void SignOut()
    {
        authenticationInstance.SignOut();
        isUserAuthenticated = false;
        logInButton.interactable = true;
        signOutButton.interactable = false;
        mainMenuScreenPanel.gameObject.SetActive(true);
        loginScreenPanel.gameObject.SetActive(false);
    }
}