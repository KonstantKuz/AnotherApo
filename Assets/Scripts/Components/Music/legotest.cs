using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class legotest : MonoBehaviour
{
    [SerializeField] private AudioClip clipToSet;
    [SerializeField] private AudioSource source;

    [ContextMenu("SetClip")]
    private void SetClip()
    {
        source.clip = clipToSet;
        source.Play();
    }
}
