using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform[] spawnPoints;

    // public IEnumerator Start()
    // {
    //     while (true)
    //     {
    //         Enemy spawnedEnemy = ObjectPooler.Instance.SpawnWeightedRandomObject(Constants.PoolGroupEnemies).GetComponent<Enemy>();
    //         spawnedEnemy.transform.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    //         spawnedEnemy.ResetEnemy();
    //         yield return new WaitForSeconds(3);
    //     }
    // }
}
