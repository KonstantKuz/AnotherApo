using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct BPMRange
{
    public int minBPM;
    public int maxBPM;
}

public class GameBeatSequencer : MonoBehaviour
{
    [SerializeField] private BPMRange bpmRange;
    
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

    private static bool isBeatedNow;
    public static bool IsBeatedNow
    {
        get { return isBeatedNow; }
    }

    public static int CurrentBeat
    {
        get { return currentBeat; }
    }

    private IEnumerator Start()
    {
        while (!GameStarter.IsGameStarted)
        {
            yield return null;
        }

        PlayerInput.OnBeatRegenerate += GenerateRandomBeats;
        
        PlayerInput.OnMusicChange += delegate
        {
            StartCoroutine(DelayedStart());
            IEnumerator DelayedStart()
            {
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                yield return new WaitForEndOfFrame();
                
                CacheAndSetBPM();
                GenerateRandomBeats();
                beatDriver.Play();
            }
        };
        
        HandleEvents();
    }

    private void CacheAndSetBPM()
    {
        currentTrackBPM = UniBpmAnalyzer.AnalyzeBpm(musicSource.clip);
        
        ClampBeat();

        beatDriver.SetBpm(currentTrackBPM);
    }

    private void ClampBeat()
    {
        if (currentTrackBPM > bpmRange.maxBPM)
        {
            if (currentTrackBPM % 2 == 0)
            {
                currentTrackBPM = bpmRange.maxBPM;
            }
            else
            {
                currentTrackBPM = bpmRange.maxBPM - 5;
            }
        }

        if (currentTrackBPM < bpmRange.minBPM)
        {
            if (currentTrackBPM % 2 == 0)
            {
                currentTrackBPM = bpmRange.minBPM;
            }
            else
            {
                currentTrackBPM = bpmRange.minBPM + 5;
            }
        }
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
        generalBeat.onAnyStep += delegate
        {
            OnBPM();
            isBeatedNow = true;
            StartCoroutine(Reset());
            IEnumerator Reset()
            {
                yield return new WaitForEndOfFrame();
                isBeatedNow = false;
            }
        };
        generalBeat.onAnyStep += delegate { currentBeat++; };
        
        durashkaBeat.onBeat += delegate { OnGeneratedBeat_Durashka(); };
        umuBeat.onBeat += delegate { OnGeneratedBeat_UMUGun(); };
    }
}
