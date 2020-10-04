using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class MusicMixer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI[] debugtexts;
    
    [SerializeField] private MusicPart[] parts;

    private IEnumerator Start()
    {
        Play();
        while (true)
        {
            yield return new WaitForSeconds(10);
            Play();
        }
    }

    [ContextMenu("PlayRandomParts")]
    private void Play()
    {
        // parts[0].PlayRandom(delegate
        // {
        //     SetDebugText(0, $"{parts[0].gameObject.name} : {parts[0].currentClip.name}");
        // });
        // parts[1].PlayRandom(delegate
        // {
        //     SetDebugText(1, $"{parts[1].gameObject.name} : {parts[1].currentClip.name}");
        // });
        parts[0].PlayRandom();
        parts[1].PlayRandom();
        
        for (int i = 2; i < parts.Length; i++)
        {
            if (Random.value > 0.5f)
            {
                parts[i].PlayRandom();
            }
            else
            {
                parts[i].Stop();
            }
        }
    }

    // public void SetDebugText(int index, string text)
    // {
    //     debugtexts[index].SetText(text);
    // }
}
