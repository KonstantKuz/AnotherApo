using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UMUGun : Gun, IDamageable
{
    [SerializeField] private Transform[] additiveBarrels;
    
    public Action OnFullDamaged;
    public int BulletCountToDie { get; private set; }

    private int currentBarrel;
    
    public void Start()
    {
        BulletCountToDie = Constants.BulletCountsToDie.UMUGun;
    }
    
    public override void Fire()
    {
        if (Time.time < nextShotTime)
        {
            return;
        }

        nextShotTime = Time.time + rateoffire;

        if (currentBarrel > additiveBarrels.Length)
        {
            currentBarrel = 0;
        } 
        
        if (currentBarrel == 0)
        {
            FireFromBarrel(barrel);
        }

        if (currentBarrel >= 1)
        {
            FireFromBarrel(additiveBarrels[currentBarrel-1]);
        }
        
        currentBarrel++;
    }

    private void FireFromBarrel(Transform actualBarrel)
    {
        ObjectPooler.Instance.SpawnObject(Constants.PoolMidFlash, actualBarrel.position, actualBarrel.rotation);

        if (trail)
        {
            ObjectPooler.Instance.SpawnObject(Constants.PoolTrail, actualBarrel.position, actualBarrel.rotation);
        }

        if (Physics.Raycast(actualBarrel.position, actualBarrel.forward, out hit, mask))
        {
            IDamageable target;
            if (hit.transform.TryGetComponent(out target))
            {
                target.TakeDamage();
            }
        }
    }

    public void TakeDamage()
    {
        BulletCountToDie--;
        if (BulletCountToDie > 0)
        {
            return;
        }

        OnFullDamaged();
        
        ObjectPooler.Instance.SpawnObject(Constants.PoolMidExplosion, transform.position + transform.forward/2);
        ObjectPooler.Instance.SpawnObject(Constants.PoolMidExplosion, transform.position - transform.forward/2);
        gameObject.SetActive(false);
    }
}