using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoCached
{
    public float speed;

    // Start is called before the first frame update
    void Awake()
    {
        _transform = transform;
    }

    public override void CustomFixedUpdate()
    {
        _transform.position += _transform.forward * speed;
    }
}
