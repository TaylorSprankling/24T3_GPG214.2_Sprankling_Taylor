using Unity.Services.Analytics;
using UnityEngine;

public static class GameAnalyticsEvents
{
    public static CustomEvent OnStartUpEvent = new CustomEvent("StartUpInformation")
    {
        // An entry requires a string for the name, then a piece of data (int, float, string, etc.)
        {"DeviceType", SystemInfo.deviceType.ToString()},
        {"DevicePlatform", Application.platform.ToString()},
    };

    public static CustomEvent OnPlayerDeathEvent = new("PlayerDeaths")
    {
        {"PlayerDeaths", 1}
    };

    public static CustomEvent OnPlayerDamagedEvent = new("PlayerDamageTaken")
    {
        {"PlayerDamageTaken", 1}
    };

    public static CustomEvent OnPlayerJumpedEvent = new("PlayerTimesJumped")
    {
        {"PlayerTimesJumped", 1}
    };

    public static CustomEvent OnEnemyKilledEvent = new("EnemiesKilled")
    {
        {"EnemiesKilled", 1}
    };
}