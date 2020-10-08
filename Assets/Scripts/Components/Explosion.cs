using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private CinemachineImpulseSource impulseSource;
    [SerializeField] private ParticleSystem light;
    [SerializeField] private ParticleSystem smoke;

    private Color lightStartColor;
    private Collider[] hitColliders;

    private ParticleSystem.MainModule mainLight;
    private ParticleSystem.MainModule mainSmoke;
    
    private void Awake()
    {
        mainLight = light.main;
        mainSmoke = smoke.main;
        lightStartColor = mainLight.startColor.color;
    }
    
    public void Explode(LayerMask mask, int damage, float radius = Constants.ExplosionRadiusSmall)
    {
        impulseSource.GenerateImpulse();

        if (damage > 0)
        {
            //mainSmoke.startColor = Color.white;
            mainLight.startColor = lightStartColor;
        }
        else
        {
            //mainSmoke.startColor = Color.green;
            mainLight.startColor = Color.green;
        }
        
        hitColliders = Physics.OverlapSphere(transform.position, radius, mask);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            if (hitColliders[i].TryGetComponent(out IDamageable damageable))
            {
                float minDamage = damage;
                float damageMultiplierByDistance =
                    //(radius * radius) / (transform.position - hitColliders[i].transform.position).sqrMagnitude;
                    radius / (transform.position - hitColliders[i].transform.position).magnitude;
                float resultDamage = minDamage * damageMultiplierByDistance;
                damageable.TakeDamage((int)resultDamage);
            }
        }
    }
}
