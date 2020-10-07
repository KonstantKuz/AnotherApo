using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameBeatSequencer : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private SequencerDriver beatDriver;

    [SerializeField] private Sequencer generalBeat;
    [SerializeField] private Sequencer durashkaBeat;
    [SerializeField] private Sequencer umuBeat;
    
    public static Action OnGeneratedBeat = delegate { };
    public static Action OnBPM = delegate { };
    public static Action OnGeneratedBeat_Durashka = delegate {  };
    public static Action OnGeneratedBeat_UMUGun = delegate {  };
    
    private int currentTrackBPM;
    private static int currentBeat;
    public static int CurrentBeat
    {
        get { return currentBeat; }
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(2);
        CacheAndSetBPM();
        GenerateRandomBeats();
        HandleEvents();
        yield return new WaitForSeconds(3);
        beatDriver.Play();
    }

    private void CacheAndSetBPM()
    {
        currentTrackBPM = UniBpmAnalyzer.AnalyzeBpm(musicSource.clip);
        beatDriver.SetBpm(currentTrackBPM);
    }

    public void GenerateRandomBeats()
    {
        Debug.Log("Generate beat");
        GenerateBeatOn(generalBeat, 0.2f, 0.2f);
        GenerateBeatOn(durashkaBeat, 0.4f, 0.2f);
        GenerateBeatOn(umuBeat, 0.2f, 0.4f);
    }

    private void GenerateBeatOn(Sequencer seq, float evenBeatChance, float unevenBeatChance)
    {
        System.Random generator = new System.Random();
        int rndInt = generator.Next(0, 100);
        float rndValue = (float)rndInt / 100f;
        seq.sequence[0] = rndValue > 0.5f;
        for (int i = 1; i < generalBeat.sequence.Length; i++)
        {
            rndInt = generator.Next(0, 100);
            rndValue = (float)rndInt / 100f;
            
            if (i % 2 == 0)
            {
                seq.sequence[i] = rndValue > evenBeatChance;
            }
            else
            {
                seq.sequence[i] = rndValue > unevenBeatChance;
            }
        }
    }

    private void HandleEvents()
    {
        generalBeat.onBeat += delegate { OnGeneratedBeat(); };
        generalBeat.onAnyStep += delegate { OnBPM(); };
        generalBeat.onAnyStep += delegate { currentBeat++; };
        
        durashkaBeat.onBeat += delegate { OnGeneratedBeat_Durashka(); };
        umuBeat.onBeat += delegate { OnGeneratedBeat_UMUGun(); };
    }
}
