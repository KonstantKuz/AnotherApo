using UnityEngine;

[System.Serializable]
public class CoverSensorsData
{
    public Transform currentCover { get; set; }
    public Transform leftSensor, rightSensor, coverHelper;
}