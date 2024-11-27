using UnityEngine;

[CreateAssetMenu(fileName = "NewAchievementItem", menuName = "Scriptable Objects/AchievementItem", order = 69)]
public class AchievementItem : ScriptableObject
{
    [SerializeField] private string displayName;
    [SerializeField] private Sprite uiSprite;
    [SerializeField] private int actionsToUnlock;
    [SerializeField] private bool goalReached = false;
    [SerializeField] private UnlockableData unlockableItemData;

    public UnlockableData UnlockableData => unlockableItemData;
    public bool GoalReached => goalReached;
    
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
}