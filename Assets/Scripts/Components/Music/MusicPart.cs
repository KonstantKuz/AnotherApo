using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class MusicPart : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    private AudioSource source;
    public AudioClip currentClip { get; private set; }

    private void Start()
    {
        source = GetComponent<AudioSource>();
    }
    
    public void PlayRandom()
    {
        StopAllCoroutines();
        StartCoroutine(DelayedPlay());
        IEnumerator DelayedPlay()
        {
            if (source.clip != null)
            {
                while (source.time < source.clip.length-0.001f)
                {
                    yield return null;
                }
            }
            
            currentClip = clips[Random.Range(0, clips.Length)];
            source.clip = currentClip;
            source.Play();
            //onClipChanged();
        }
    }

    public void Stop()
    {
        source.Stop();
    }
}
