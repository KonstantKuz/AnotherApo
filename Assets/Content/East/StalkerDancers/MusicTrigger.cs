using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    private float vol;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vol = other.GetComponentInChildren<AudioSource>().volume;
            other.GetComponentInChildren<AudioSource>().volume = 0;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponentInChildren<AudioSource>().volume = vol;
        }
    }
}
