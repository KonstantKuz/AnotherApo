using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    private Collider[] hitColliders;
    
    public void Explode(LayerMask mask, int damage, float radius = Constants.ExplosionRadius)
    {
        impulseSource.GenerateImpulse(transform.position - Camera.main.transform.position);
        
        hitColliders = Physics.OverlapSphere(transform.position, radius, mask);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].TryGetComponent(out IDamageable damageable))
            {
                float minDamage = damage;
                float damageMultiplierByDistance =
                    radius / (transform.position - hitColliders[i].transform.position).magnitude;
                float resultDamage = minDamage * damageMultiplierByDistance;
                damageable.TakeDamage((int)resultDamage);
            }
        }
    }
}
