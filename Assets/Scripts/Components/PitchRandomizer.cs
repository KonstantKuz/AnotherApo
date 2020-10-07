using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchRandomizer : MonoBehaviour
{
    [SerializeField] private Vector2 range = new Vector2(0.8f, 1.2f);
    [SerializeField] private AudioSource source;

    private void OnEnable()
    {
        source.pitch = Random.Range(range.x, range.y);
    }
}
