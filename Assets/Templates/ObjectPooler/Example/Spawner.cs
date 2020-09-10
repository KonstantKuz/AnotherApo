using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private IEnumerator Start()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.5f);
            ObjectPooler.Instance.SpawnObject("1", transform.position + Vector3.up * Random.Range(-3, 3) + Vector3.right * Random.Range(-3, 3));
            Debug.Log("Spawned concrete object 1!");
            yield return new WaitForSeconds(0.5f);
            ObjectPooler.Instance.SpawnObject("2", transform.position + Vector3.up * Random.Range(-3, 3) + Vector3.right * Random.Range(-3, 3));
            Debug.Log("Spawned concrete object 2!");

            yield return new WaitForSeconds(0.5f);
            ObjectPooler.Instance.SpawnObject("3", transform.position + Vector3.up * Random.Range(-3, 3) + Vector3.right * Random.Range(-3, 3));
            Debug.Log("Spawned concrete object 3!");

            yield return new WaitForSeconds(1f);
            ObjectPooler.Instance.SpawnRandomObject("cubes", transform.position + Vector3.up * Random.Range(-3, 3) + Vector3.right * Random.Range(-3, 3));
            ObjectPooler.Instance.SpawnRandomObject("cubes", transform.position + Vector3.up * Random.Range(-3, 3) + Vector3.right * Random.Range(-3, 3));
            ObjectPooler.Instance.SpawnRandomObject("cubes", transform.position + Vector3.up * Random.Range(-3, 3) + Vector3.right * Random.Range(-3, 3));
            Debug.Log("Spawned three random objects from group cubes!");
        }
    }
}
