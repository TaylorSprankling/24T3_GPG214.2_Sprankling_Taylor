using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class EnemyKilledAnalytic : MonoBehaviour
{
    public void RecordEnemyKilled()
    {
        AnalyticsService.Instance.RecordEvent(GameAnalyticsEvents.OnEnemyKilledEvent);
        Debug.Log("Recorded enemy killed event");
    }
}