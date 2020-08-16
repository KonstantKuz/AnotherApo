using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoCached
{
    public Transform barrel;
    public float rateoffire;

    private float lastShotTime;

    private Ray ray;
    private RaycastHit hit;

    public void Fire()
    {
        if (Time.time > lastShotTime)
        {
            lastShotTime += rateoffire;
            //Spawn(barrel.position + Random.Range(positionJitter.x, positionJitter.y) * barrel.forward, barrel.rotation);
        }
        ray.origin = barrel.position;
        ray.direction = barrel.forward;
        if(Physics.Raycast(ray, out hit))
        {

        }
    }
}
