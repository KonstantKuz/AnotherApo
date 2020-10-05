using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class UMUGun : Gun, IDamageable
{
    [SerializeField] private Transform[] additiveBarrels;
    
    public Action OnFullDamaged;
    public int TotalHealth { get; private set; }

    private int currentBarrel;
    
    public void Start()
    {
        SetDamageValue(Constants.DamagePerHit.UMUGun);
        TotalHealth = Constants.TotalHealth.UMUGun;
    }

    public void SubscribeToBeat()
    {
        GameBeatSequencer.OnGeneratedBeat_UMUGun += Fire;
    }

    private void UnsubscribeFromBeat()
    {
        GameBeatSequencer.OnGeneratedBeat_UMUGun -= Fire;
    }

    public override void Fire()
    {
        // if (Time.time < nextShotTime)
        // {
        //     return;
        // }
        // nextShotTime = Time.time + rateoffire;

        SpawnEffects();
        UpdateCurrentBarrel();
        HandleHit();
    }

    private void SpawnEffects()
    {
        if (currentBarrel == 0)
        {
            SpawnFlash(actualCaster.transform);
            TrySpawnTrail(actualCaster.transform);
        }

        if (currentBarrel >= 1)
        {
            SpawnFlash(additiveBarrels[currentBarrel - 1]);
            TrySpawnTrail(additiveBarrels[currentBarrel - 1]);
        }
    }
    
    private void UpdateCurrentBarrel()
    {
        currentBarrel++;

        if (currentBarrel > additiveBarrels.Length)
        {
            currentBarrel = 0;
        }
    }

    public void TakeDamage(int value)
    {
        TotalHealth -= value;
        if (TotalHealth > 0)
        {
            return;
        }
        
        UnsubscribeFromBeat();
        OnFullDamaged();
        
        ObjectPooler.Instance.SpawnObject(Constants.PoolExplosionMid, transform.position + transform.forward/2);
        ObjectPooler.Instance.SpawnObject(Constants.PoolExplosionMid, transform.position - transform.forward/2);
        gameObject.SetActive(false);
    }
}