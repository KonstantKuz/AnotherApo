using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Gun : MonoCached
{
    [SerializeField] private bool trail;
    [SerializeField] private Transform barrel;
    [SerializeField] private float rateoffire;
    private float lastShotTime;

    public void Fire()
    {
        if (!(Time.time > lastShotTime))
        {
            return;
        }

        lastShotTime = Time.time + rateoffire;
        ObjectPooler.Instance.SpawnObject(Constants.PoolFlash, barrel.position, barrel.rotation);
        
        if (trail)
        {
            ObjectPooler.Instance.SpawnObject(Constants.PoolTrail, barrel.position, barrel.rotation);
        }
    }
}
