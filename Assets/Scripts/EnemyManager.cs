using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    private List<EnemyPatrolTopDown> enemies = new List<EnemyPatrolTopDown>();
    private float globalSpeedMultiplier = 1f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void RegisterEnemy(EnemyPatrolTopDown enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
        ApplySpeed(enemy);
    }

    public void UnregisterEnemy(EnemyPatrolTopDown enemy)
    {
        if (enemies.Contains(enemy))
            enemies.Remove(enemy);
    }

    public void SetGlobalSpeedMultiplier(float multiplier)
    {
        globalSpeedMultiplier = multiplier;
        foreach (var e in enemies)
            ApplySpeed(e);
    }

    private void ApplySpeed(EnemyPatrolTopDown enemy)
    {
        if (enemy != null)
            enemy.speed = enemy.originalSpeed * globalSpeedMultiplier;
    }
}
