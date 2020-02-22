using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoCached
{
    public Transform barrel;
    public float rateoffire;
    public ObjectPoolerBase bulletPooler;

    public Vector2 positionJitter;

    private float lastShotTime;

    private Ray ray;
    private RaycastHit hit;

    private void Start()
    {
        _transform = transform;
        bulletPooler.AwakePooler();
    }

    public void Fire()
    {
        if (Time.time > lastShotTime)
        {
            lastShotTime += rateoffire;
            bulletPooler.SpawnRandom(barrel.position + Random.Range(positionJitter.x, positionJitter.y) * barrel.forward, barrel.rotation);
        }
        ray.origin = barrel.position;
        ray.direction = barrel.forward;
        if(Physics.Raycast(ray, out hit))
        {

        }
    }
}
