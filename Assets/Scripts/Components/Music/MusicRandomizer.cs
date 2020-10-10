using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
        
        PlayerInput.OnMusicChange += PlayRandomMusic;
        PlayRandomMusic();

        source.volume = 0;
        while (source.volume < 0.5f)
        {
            source.volume += Time.deltaTime*0.01f;
            yield return new WaitForEndOfFrame();
        }
    }

    private void PlayRandomMusic()
    {
        System.Random generator = new System.Random();
        int rndIndex = generator.Next(0, music.Length);
        source.clip = music[rndIndex];
        source.time = Random.Range(source.clip.length / 10, source.clip.length / 6);
        source.Play();
    }

    private void Update()
    {
        UpdateMusicVolume();
        UpdateMasterVolume();
    }

    private void UpdateMusicVolume()
    {
        source.outputAudioMixerGroup.audioMixer.GetFloat(
            Constants.MusicVolParam, 
            out float musicVolume);
        
        if (Input.GetKeyDown(KeyCode.P))
        {
            musicVolume += 0.2f;
            SetVolume(Constants.MusicVolParam, musicVolume);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            musicVolume -= 0.2f;
            SetVolume(Constants.MusicVolParam, musicVolume);
        }
    }

    private void UpdateMasterVolume()
    {
        source.outputAudioMixerGroup.audioMixer.GetFloat(
                   Constants.MasterVolume, 
                   out float masterVolume);
        
        if (Input.GetKeyDown(KeyCode.L))
        {
            masterVolume += 0.2f;
            SetVolume(Constants.MasterVolume, masterVolume);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            masterVolume -= 0.2f;
            SetVolume(Constants.MasterVolume, masterVolume);
        }
    }

    private void SetVolume(string param, float volume)
    {
        source.outputAudioMixerGroup.audioMixer.SetFloat(param, volume);
    }
}
