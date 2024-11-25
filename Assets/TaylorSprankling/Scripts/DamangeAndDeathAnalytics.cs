using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class DamangeAndDeathAnalytics : MonoBehaviour
{
    public void RecordDamage()
    {
        AnalyticsService.Instance.RecordEvent(GameAnalyticsEvents.OnPlayerDamagedEvent);
        Debug.Log("Recorded player damage event");
    }

    public void RecordDeath()
    {
        AnalyticsService.Instance.RecordEvent(GameAnalyticsEvents.OnPlayerDeathEvent);
        Debug.Log("Recorded player death event");
    }
}