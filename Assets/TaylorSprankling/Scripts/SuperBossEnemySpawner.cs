using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperBossEnemySpawner : MonoBehaviour
{
    [SerializeField] private UnlockableItem superBossEnemyUnlockable;
    private bool hasSpawnedEnemy = false;

    void Update()
    {
        if (superBossEnemyUnlockable.UnlockableData.IsUnlocked && !hasSpawnedEnemy)
        {
            Instantiate(superBossEnemyUnlockable.ItemUnlockedPrefab, transform.position, transform.rotation);
            hasSpawnedEnemy = true;
        }
    }
}
