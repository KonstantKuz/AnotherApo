using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Gun : MonoCached
{
    [SerializeField] private bool trail;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] private CrossHairCaster crossHairCaster;
    [SerializeField] private LineRenderer lazer;
    [SerializeField] private Transform barrel;
    [SerializeField] private float rateoffire;
    private float lastShotTime;

    public override void OnEnable()
    {
        base.OnEnable();
        crossHairCaster.OnCrossHairUpdated += OnCrossHairUpdated;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        crossHairCaster.OnCrossHairUpdated -= OnCrossHairUpdated;
    }

    private void OnCrossHairUpdated(Vector3 crossHairPosition)
    {
        lazer.SetPosition(0, barrel.position);
        lazer.SetPosition(1, crossHairPosition);
    }

    public void Fire()
    {
        // if (muzzleFlash == null)
        // {
        //     muzzleFlash = ObjectPooler.Instance.SpawnObject("MuzzleFlash");c
        //     muzzleFlash.transform.parent = barrel;
        //     muzzleFlash.transform.localPosition = Vector3.zero;
        //     muzzleFlash.transform.localRotation = quaternion.Euler(Vector3.zero);
        // }
        
        if (Time.time > lastShotTime)
        {
            lastShotTime = Time.time + rateoffire;
            //PlayMuzzleFlash();
            if (trail)
            {
                ObjectPooler.Instance.SpawnObject("Trail", barrel.position, barrel.rotation);
                ObjectPooler.Instance.SpawnObject("MuzzleFlash", barrel.position, barrel.rotation);
            }
        }
    }

    private void PlayMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        StartCoroutine(delayedDisable());
        IEnumerator delayedDisable()
        {
            yield return new WaitForSeconds(rateoffire-rateoffire/2);
            muzzleFlash.SetActive(false);
        }
    }
}
