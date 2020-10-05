using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrossHairViewer : MonoBehaviour
{
    [SerializeField] private RectTransform crossHair;
    [SerializeField] private CrossHairCaster crossHairCaster;
    
    private void OnEnable()
    {
        crossHairCaster.OnCrossHairUpdated += SetCrossHairPosition;
    }

    private void SetCrossHairPosition(Vector3 targetWorldPosition)
    {
        Vector3 targetScreenPosition = Camera.main.WorldToScreenPoint(targetWorldPosition);
        crossHair.transform.position = Vector3.Lerp(crossHair.transform.position, targetScreenPosition, 
                                                    Time.deltaTime * 2f);
    }
}
