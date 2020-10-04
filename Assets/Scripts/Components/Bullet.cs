using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoCached
{
    public float speed;

    public override void CustomUpdate()
    {
        transform.position += transform.forward * speed;
    }
}
