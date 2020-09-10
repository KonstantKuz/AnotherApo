using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoCached
{
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
        if (Time.time > lastShotTime)
        {
            lastShotTime = Time.time + rateoffire;
            PlayMuzzleFlash();
            //Spawn(barrel.position + Random.Range(positionJitter.x, positionJitter.y) * barrel.forward, barrel.rotation);
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
