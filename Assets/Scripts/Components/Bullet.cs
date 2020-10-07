using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoCached
{
    public float speed;

    public override void CustomFixedUpdate()
    {
        if (Physics.Raycast(transform.position, transform.forward, 2f))
        {
            ObjectPooler.Instance.ReturnObject(gameObject, gameObject.name);
        }
        transform.position += transform.forward * speed;
    }
}
