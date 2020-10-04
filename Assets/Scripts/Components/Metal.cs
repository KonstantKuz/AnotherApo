using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metal : MonoCached, IHitMaterial
{
    public void SpawnHitReaction(Vector3 position, Vector3 normal)
    {
        Transform hitEffect =ObjectPooler.Instance.SpawnObject(Constants.PoolHitMetal).transform;
        hitEffect.position = position;
        hitEffect.forward = normal;
    }
}
