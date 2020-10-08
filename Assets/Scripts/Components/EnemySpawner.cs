using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Vector2 spawnPeriodRange;
    [SerializeField] private Transform[] spawnPoints;
    private UMUBot currentUmu;

    private int currentEnemyCount;
    private int maxEnemyCount = 30;
    public IEnumerator Start()
    {
        while (!GameStarter.IsGameStarted)
        {
            yield return null;
        }
        
        FirstSpawn();
        while (true)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(Random.Range(spawnPeriodRange.x, spawnPeriodRange.y));
        }
    }

    private void SpawnEnemy()
    {
        if (currentEnemyCount > maxEnemyCount)
            return;
        
        Enemy spawnedEnemy = ObjectPooler.Instance.SpawnWeightedRandomObject(Constants.PoolGroupEnemies)
                                         .GetComponent<Enemy>();
        spawnedEnemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        TryGetUMU(spawnedEnemy);

        currentEnemyCount++;
        
        SubScribeToEnemyDeath(spawnedEnemy);
    }

    private void SubScribeToEnemyDeath(Enemy enemy)
    {
        enemy.OnDeath += delegate { DecreaseCurrentEnemyCount(enemy); };
    }

    private void UnsubscribeFromEnemyDeath(Enemy enemy)
    {
        enemy.OnDeath -= delegate { DecreaseCurrentEnemyCount(enemy); };
    }

    private void DecreaseCurrentEnemyCount(Enemy enemy)
    {
        currentEnemyCount--;
        UnsubscribeFromEnemyDeath(enemy);
    }

    private void FirstSpawn()
    {
        for (int i = 0; i < 10; i++)
        {
            SpawnEnemy();
        }
    }

    private void TryGetUMU(Enemy spawnedEnemy)
    {
        UMUBot umuBot = spawnedEnemy as UMUBot;
        if (umuBot == null)
        {
            return;
        }
        
        if (currentUmu != null)
        {
            ObjectPooler.Instance.ReturnObject(umuBot.gameObject, umuBot.gameObject.name);
            return;
        }
        
        currentUmu = umuBot;
        
        SubscribeToUMUDeath();
    }

    private void SubscribeToUMUDeath()
    {
        currentUmu.OnDeath += ResetCurrentUMU;
    }

    private void UnsubscribeFromUMUDeath()
    {
        currentUmu.OnDeath -= ResetCurrentUMU;
    }

    private void ResetCurrentUMU()
    {
        UnsubscribeFromUMUDeath();
        currentUmu = null;
    }
}
