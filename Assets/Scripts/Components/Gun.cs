using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Gun : MonoCached
{
    [SerializeField] protected bool trail;
    [SerializeField] protected LayerMask mask;
    [SerializeField] protected Transform barrel;
    [SerializeField] protected float rateoffire;
    protected float nextShotTime;
    protected RaycastHit hit;
    
    public virtual void Fire()
    {
        if (Time.time < nextShotTime)
        {
            return;
        }

        nextShotTime = Time.time + rateoffire;
        ObjectPooler.Instance.SpawnObject(Constants.PoolSmallFlash, barrel.position, barrel.rotation);
        
        if (trail)
        {
            ObjectPooler.Instance.SpawnObject(Constants.PoolTrail, barrel.position, barrel.rotation);
        }

        if (Physics.Raycast(barrel.position, barrel.forward, out hit, mask))
        {
            IDamageable target;
            if (hit.transform.TryGetComponent(out target))
            {
                target.TakeDamage();
            }
        }
    }
}
