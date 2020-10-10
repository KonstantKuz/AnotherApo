using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class UMUGun : Gun, IDamageable
{
    [SerializeField] private Transform[] additiveBarrels;
    
    public Action OnFullDamaged;
    public int TotalHealth { get; private set; }

    private int currentBarrel;
    private GameObject smokeStream;
    
    public void ResetGun()
    {
        gameObject.SetActive(true);
        SetDamageValue(Constants.DamagePerHit.UMUGun);
        TotalHealth = Constants.TotalHealth.UMUGun;
        SubscribeToBeat();
    }

    private void SubscribeToBeat()
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
            SpawnFlash(actualCaster.transform, Constants.PoolFlashMid);
            TrySpawnTrail(actualCaster.transform, Constants.PoolBulletTrailBig);
        }

        if (currentBarrel >= 1)
        {
            SpawnFlash(additiveBarrels[currentBarrel - 1], Constants.PoolFlashMid);
            TrySpawnTrail(additiveBarrels[currentBarrel - 1], Constants.PoolBulletTrailBig);
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
        TrySpawnDamagedSignalEffect();
        
        if (TotalHealth > 0)
        {
            return;
        }

        StartCoroutine(ExplodeOnBeat());
        IEnumerator ExplodeOnBeat()
        {
            while (!GameBeatSequencer.IsBeatedNow)
            {
                yield return null;
            }
            
            UnsubscribeFromBeat();
            OnFullDamaged();
            SpawnHealExplosions();
            ReturnDamagedSignalToPool();
            gameObject.SetActive(false);
        }
    }

    private void TrySpawnDamagedSignalEffect()
    {
        if (TotalHealth < Constants.TotalHealth.UMUGun * 0.3f && smokeStream == null)
        {
            smokeStream =
                ObjectPooler.Instance.SpawnObject(Constants.PoolSmokeStream, transform.position, Quaternion.identity);
            smokeStream.transform.SetParent(transform);
        }
    }

    private void SpawnHealExplosions()
    {
        Explosion explosion = ObjectPooler.Instance.SpawnObject(Constants.PoolExplosionMid).GetComponent<Explosion>();
        explosion.transform.position = transform.position + transform.forward / 2;
        explosion.Explode(1 << LayerMasks.Player, Constants.HealPerExplosion.UMUGun);
    }

    private void ReturnDamagedSignalToPool()
    {
        smokeStream.transform.SetParent(null);
        ObjectPooler.Instance.ReturnObject(smokeStream.gameObject, smokeStream.gameObject.name);
    }
}