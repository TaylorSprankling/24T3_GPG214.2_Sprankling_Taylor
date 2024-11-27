using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementsAndUnlockables : MonoBehaviour
{
    [SerializeField] private List<AchievementItem> allAchievementItems = new();
    [SerializeField] private List<UnlockableItem> allUnlockables = new();

    [SerializeField] private string testAchievementID;
    [SerializeField] private string testUnlockableID;

    private IEnumerator Start()
    {
        yield return StartCoroutine(LoadAllAchievements());
        yield return StartCoroutine(LoadAllUnlockables());
        yield return null;
    }

    private void Update()
    {
        // Debug keys to test achievements and unlockables
        if (Input.GetKeyDown(KeyCode.I))
        {
            UpdateAchievement(FindAchievement(testAchievementID));
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpdateUnlockable(FindUnlockable(testUnlockableID));
        }
    }

    private AchievementItem FindAchievement(string unlockableID)
    {
        for (int i = 0; i < allAchievementItems.Count; i++)
        {
            if (allAchievementItems[i].UnlockableData.UnlockableID == unlockableID)
            {
                return allAchievementItems[i];
            }
        }
        return null;
    }

    public void UpdateAchievement(AchievementItem achievement)
    {
        if (!achievement.UnlockableData.IsUnlocked)
        {
            achievement.UpdateGoal();

            if (achievement.GoalReached)
            {
                achievement.UnlockableData.Unlock();

                if (!achievement.UnlockableData.HasPreviouslyBeenDisplayed)
                {
                    // Show UI achievement get
                    achievement.UnlockableData.UpdateHasBeenDisplayed();
                }

            }
            if (Application.internetReachability != NetworkReachability.NotReachable || (FirebaseAuth.DefaultInstance.CurrentUser != null && !string.IsNullOrEmpty(FirebaseAuth.DefaultInstance.CurrentUser.UserId)))
            {
                StartCoroutine(achievement.UnlockableData.SaveData(FirebaseDatabase.DefaultInstance.RootReference, FirebaseAuth.DefaultInstance.CurrentUser.UserId));
            }
        }
    }

    private UnlockableItem FindUnlockable(string unlockableID)
    {
        for (int i = 0; i < allUnlockables.Count; i++)
        {
            if (allUnlockables[i].UnlockableData.UnlockableID == unlockableID)
            {
                return allUnlockables[i];
            }
        }
        return null;
    }

    public void UpdateUnlockable(UnlockableItem unlockable)
    {
        if (!unlockable.UnlockableData.IsUnlocked)
        {
            unlockable.UpdateGoal();

            if (unlockable.GoalReached)
            {
                unlockable.UnlockableData.Unlock();

                if (!unlockable.UnlockableData.HasPreviouslyBeenDisplayed)
                {
                    // Show UI achievement get
                    unlockable.UnlockableData.UpdateHasBeenDisplayed();
                }
            }

            if (Application.internetReachability != NetworkReachability.NotReachable || (FirebaseAuth.DefaultInstance.CurrentUser != null && !string.IsNullOrEmpty(FirebaseAuth.DefaultInstance.CurrentUser.UserId)))
            {
                StartCoroutine(unlockable.UnlockableData.SaveData(FirebaseDatabase.DefaultInstance.RootReference, FirebaseAuth.DefaultInstance.CurrentUser.UserId));
            }
        }
    }

    private IEnumerator LoadAllAchievements()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable || (FirebaseAuth.DefaultInstance.CurrentUser != null && !string.IsNullOrEmpty(FirebaseAuth.DefaultInstance.CurrentUser.UserId)))
        {
            for (int i = 0; i < allAchievementItems.Count; i++)
            {
                yield return StartCoroutine(allAchievementItems[i].UnlockableData.LoadData(FirebaseDatabase.DefaultInstance.RootReference, FirebaseAuth.DefaultInstance.CurrentUser.UserId));
                yield return null;
            }
        }
        yield return null;
    }

    private IEnumerator LoadAllUnlockables()
    {
        if (Application.internetReachability != NetworkReachability.NotReachable || (FirebaseAuth.DefaultInstance.CurrentUser != null && !string.IsNullOrEmpty(FirebaseAuth.DefaultInstance.CurrentUser.UserId)))
        {
            for (int i = 0; i < allUnlockables.Count; i++)
            {
                yield return StartCoroutine(allUnlockables[i].UnlockableData.LoadData(FirebaseDatabase.DefaultInstance.RootReference, FirebaseAuth.DefaultInstance.CurrentUser.UserId));
                yield return null;
            }
        }
        yield return null;
    }
}