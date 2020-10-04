using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEventsRepeater : MonoBehaviour
{
    public Action<string> OnEventRecieved;
    
    public void Execute(string value)
    {
        OnEventRecieved(value);
    }
}
