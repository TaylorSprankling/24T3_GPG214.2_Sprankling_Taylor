using UnityEngine;

[CreateAssetMenu(fileName = "NewUnlockableItem", menuName = "Scriptable Objects/UnlockableItem", order = 69)]
public class UnlockableItem : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite uiSprite;
    [SerializeField] private GameObject itemUnlockedPrefab;
    [SerializeField] private int actionsToUnlock = 1;
    [SerializeField] private bool goalReached = false;
    [SerializeField] private UnlockableData unlockableItemData;


    public UnlockableData UnlockableData => unlockableItemData;
    public bool GoalReached => goalReached;
    public GameObject ItemUnlockedPrefab => itemUnlockedPrefab;

    public void UpdateGoal()
    {
        if (!goalReached)
        {
            unlockableItemData.CurrentActionsRecorded++;
            if (unlockableItemData.CurrentActionsRecorded == actionsToUnlock)
            {
                goalReached = true;
            }
        }
    }

    // Save and unlock logic should have been in here :(
}