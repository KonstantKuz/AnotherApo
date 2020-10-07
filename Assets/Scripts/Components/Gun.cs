using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Gun : MonoCached
{
    [SerializeField] protected LayerMask mask;
    [SerializeField] protected bool trail;
    [SerializeField] protected CrossHairCaster actualCaster;
    // [SerializeField] protected float rateoffire;
    // protected float nextShotTime;
    protected int damage;

    public void SetDamageValue(int value)
    {
        damage = value;
    }
    
    public virtual void Fire()
    {
        // if (Time.time < nextShotTime)
        // {
        //     return;
        // }
        // nextShotTime = Time.time + rateoffire;

        SpawnFlash(actualCaster.transform);
        TrySpawnTrail(actualCaster.transform);
        HandleHit();
    }

    protected void SpawnFlash(Transform barrel)
    {
        ObjectPooler.Instance.SpawnObject(Constants.PoolFlashSmall, barrel.position, barrel.rotation);
    }

    protected void TrySpawnTrail(Transform barrel)
    {
        if (trail)
        {
            ObjectPooler.Instance.SpawnObject(Constants.PoolBulletTrail, barrel.position, barrel.rotation);
        }
    }

    protected void HandleHit()
    {
        if (actualCaster.Hit.transform == null)
            return;
        
        CheckForDamageable();
        CheckForHitReaction();
    }

    private void CheckForDamageable()
    {
        if (mask.Contains(actualCaster.Hit.transform.gameObject.layer))
        {
            if (actualCaster.Hit.transform.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage);
            }
        }
    }

    private void CheckForHitReaction()
    {
        for (int i = 0; i < actualCaster.AllHits.Length; i++)
        {
            IHitMaterial[] materials = actualCaster.AllHits[i].transform.GetComponents<IHitMaterial>();
            if (materials != null)
            {
                for (int j = 0; j < materials.Length; j++)
                {
                    materials[j].SpawnHitReaction(actualCaster.AllHits[i].point, actualCaster.AllHits[i].normal);
                }
            }
        }
        
        // IHitMaterial[] materials = actualCaster.Hit.transform.GetComponents<IHitMaterial>();
        // if (materials != null)
        // {
        //     for (int i = 0; i < materials.Length; i++)
        //     {
        //         materials[i].SpawnHitReaction(actualCaster.Hit.point, actualCaster.Hit.normal);
        //     }
        // }
        
        // if (actualCaster.Hit.transform.TryGetComponent(out IHitMaterial material))
        // {
        //     material.SpawnHitReaction(actualCaster.Hit.point, actualCaster.Hit.normal);
        // }
        
        if (actualCaster.Hit.collider.gameObject.layer == LayerMasks.Ground)
        {
            Transform groundHit = ObjectPooler.Instance.SpawnObject(Constants.PoolHitGroundSmall).transform;
            groundHit.transform.position = actualCaster.Hit.point;
            groundHit.forward = actualCaster.Hit.normal;
        }
    }
}
