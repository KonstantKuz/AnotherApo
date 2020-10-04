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
    protected int damage;

    public void SetDamageValue(int value)
    {
        damage = value;
    }
    
    public virtual void Fire()
    {
        if (Time.time < nextShotTime)
        {
            return;
        }

        nextShotTime = Time.time + rateoffire;
        ObjectPooler.Instance.SpawnObject(Constants.PoolFlashSmall, barrel.position, barrel.rotation);
        
        if (trail)
        {
            ObjectPooler.Instance.SpawnObject(Constants.PoolBulletTrail, barrel.position, barrel.rotation);
        }

        if (Physics.Raycast(barrel.position, barrel.forward, out hit, mask))
        {
            IDamageable target;
            if (hit.transform.TryGetComponent(out target))
            {
                target.TakeDamage(damage);
            }

            IHitMaterial material;
            if (hit.transform.TryGetComponent(out material))
            {
                material.SpawnHitReaction(hit.point, hit.normal);
            }
            
            CheckForGround();
        }
    }

    public void CheckForGround()
    {
        if (hit.collider.gameObject.layer == LayerMasks.Ground)
        {
            Transform groundHit = ObjectPooler.Instance.SpawnObject(Constants.PoolHitGroundSmall).transform;
            groundHit.transform.position = hit.point;
            groundHit.forward = hit.normal;
        } 
    }
}
