using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int enemiesCount;
    [SerializeField] private Transform[] spawnPoints;
    private UMUBot currentUmu;

    private IEnumerator Start()
    {
        while (!GameStarter.IsGameStarted)
        {
            yield return null;
        }
        
        FirstSpawn();
    }

    private void FirstSpawn()
    {
        for (int i = 0; i < enemiesCount; i++)
        {
            SpawnEnemy();
        }
    }
    
    private void SpawnEnemy()
    {
        Enemy spawnedEnemy = ObjectPooler.Instance.SpawnWeightedRandomObject(Constants.PoolGroupEnemies)
                                         .GetComponent<Enemy>();
        spawnedEnemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        
        SubscribeToEnemyDeath(spawnedEnemy);
        Clamp1_UMUsCount(spawnedEnemy);
    }

    private void SubscribeToEnemyDeath(Enemy enemy)
    {
        if(enemy.OnDeath != null)
            return;
        
        enemy.OnDeath += SpawnEnemy;
    }

    private void Clamp1_UMUsCount(Enemy spawnedEnemy)
    {
        UMUBot umuBot = spawnedEnemy as UMUBot;
        if (umuBot == null)
        {
            return;
        }
        
        if (currentUmu != null && currentUmu.gameObject.activeInHierarchy)
        {
            ObjectPooler.Instance.ReturnObject(umuBot.gameObject, umuBot.gameObject.name);
            return;
        }
        
        currentUmu = umuBot;
    }
}
