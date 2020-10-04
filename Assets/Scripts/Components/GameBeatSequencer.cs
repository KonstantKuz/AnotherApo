using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameBeatSequencer : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private SequencerDriver beatDriver;

    public static Action OnGeneratedBeat = delegate { };
    public static Action OnBPM = delegate { };

    private Sequencer beatSequencer;
    private int currentTrackBPM;
    
    private IEnumerator Start()
    {
        CacheAndSetBPM();
        GenerateRandomBeat();
        HandleEvents();
        yield return new WaitForSeconds(2);
        Play();
    }

    private void CacheAndSetBPM()
    {
        currentTrackBPM = UniBpmAnalyzer.AnalyzeBpm(musicSource.clip);
        beatDriver.SetBpm(currentTrackBPM);
    }

    private void GenerateRandomBeat()
    {
        beatSequencer = beatDriver.sequencers[0] as Sequencer;

        beatSequencer.sequence[0] = Random.value > 0.5f;
        for (int i = 1; i < beatSequencer.sequence.Length; i++)
        {
            if (!beatSequencer.sequence[0] && i%2 == 0)
                beatSequencer.sequence[i] = Random.value > 0.5f;
        }
    }

    private void HandleEvents()
    {
        Sequencer sequencer = beatDriver.sequencers[0] as Sequencer;

        sequencer.onBeat += delegate { OnGeneratedBeat(); };
        sequencer.onAnyStep += delegate { OnBPM(); };
    }

    [ContextMenu("PLAY")]
    private void Play()
    {
        musicSource.Play();
        beatDriver.Play();
    }
}
