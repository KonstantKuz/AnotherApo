using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float UMUSpawnDelay = 10f;
    [SerializeField] private int durashkaCount = 10;
    [SerializeField] private int spiderAstCount = 10;
    [SerializeField] private Transform[] spawnPoints;
    private UMUBot currentUmu;

    private IEnumerator Start()
    {
        while (!GameStarter.IsGameStarted)
        {
            yield return null;
        }

        SpawnAllDurashkas();
        SpawnAllSpiderAsts();
        
        yield return new WaitForSeconds(UMUSpawnDelay);

        SpawnUMU();
    }

    private void SpawnUMU()
    {
        UMUBot umu = ObjectPooler.Instance.SpawnObject(Constants.PoolUMUBot).GetComponent<UMUBot>();
        umu.transform.position = RandomPosition();

        if (umu.OnDeath == null)
        {
            umu.OnDeath += SpawnUMU;
        }
    }

    private void SpawnAllSpiderAsts()
    {
        for (int i = 0; i < spiderAstCount; i++)
        {
            SpawnSpiderAst();
        }
    }

    private void SpawnSpiderAst()
    {
        SpiderAst spiderAst = ObjectPooler.Instance.SpawnObject(Constants.PoolSpiderAst).GetComponent<SpiderAst>();
        spiderAst.transform.position = RandomPosition();

        if (spiderAst.OnDeath == null)
        {
            spiderAst.OnDeath += SpawnSpiderAst;
        }
    }

    private void SpawnAllDurashkas()
    {
        for (int i = 0; i < durashkaCount; i++)
        {
            SpawnDurashka();
        }
    }

    private void SpawnDurashka()
    {
        Durashka durashka = ObjectPooler.Instance.SpawnObject(Constants.PoolDurashka).GetComponent<Durashka>();
        durashka.transform.position = RandomPosition();

        if (durashka.OnDeath == null)
        {
            durashka.OnDeath += SpawnDurashka;
        }
    }
    private Vector3 RandomPosition()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }
}
