using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicRandomizer : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip[] music;

    private IEnumerator Start()
    {
        while (!GameStarter.IsGameStarted)
        {
            yield return null;
        }
        
        System.Random generator = new System.Random();
        int rndIndex = generator.Next(0, music.Length);
        source.clip = music[rndIndex];
        source.time = Random.Range(source.clip.length / 8, source.clip.length / 4);
        source.volume = 0;
        source.Play();
        while (source.volume < 0.5f)
        {
            source.volume += Time.deltaTime*0.5f;
            yield return null;
        }
    }
}
