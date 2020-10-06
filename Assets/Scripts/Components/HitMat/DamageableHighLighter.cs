using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DamageableHighLighter : MonoCached, IHitMaterial
{
    [SerializeField] private Renderer[] highLightableRenderers;

    private int highLightCount;
    private List<Color> startColors;

    private void Start()
    {
        startColors = new List<Color>();
        for (int i = 0; i < highLightableRenderers.Length; i++)
        {
            highLightableRenderers[i].material.EnableKeyword(Constants.EmissionKeyword);
            startColors.Add(highLightableRenderers[i].material.GetColor(Constants.EmissionProperty));
        }
    }
    
    public void SpawnHitReaction(Vector3 position, Vector3 normal)
    {
        if (highLightCount > 3)
            return;
        
        StartCoroutine(HighLight());
        IEnumerator HighLight()
        {
            for (int i = 0; i < highLightableRenderers.Length; i++)
            {
                highLightableRenderers[i].material.DOColor(Color.red/2, Constants.EmissionProperty, 0.5f);
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < highLightableRenderers.Length; i++)
            {
                highLightableRenderers[i].material.DOColor(startColors[i], Constants.EmissionProperty, 0.5f);
            }
            yield return new WaitForSeconds(0.5f);
            highLightCount++;
        }
    }
}
