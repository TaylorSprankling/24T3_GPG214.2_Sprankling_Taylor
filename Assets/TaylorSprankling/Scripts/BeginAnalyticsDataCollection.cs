using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class BeginAnalyticsDataCollection : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return UnityServices.InitializeAsync();

        // Start data collection
        AnalyticsService.Instance.StartDataCollection();

        // Send my event to the service/Unity analytics dashboard
        AnalyticsService.Instance.RecordEvent(GameAnalyticsEvents.OnStartUpEvent);
        Debug.Log("On start up event called");
        yield return null;
    }
}

// Stop all data collection
//AnalyticsService.Instance.StopDataCollection();

// Delete saved data
//AnalyticsService.Instance.RequestDataDeletion();