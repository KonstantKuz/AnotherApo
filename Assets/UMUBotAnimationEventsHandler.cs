using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UMUBotAnimationEventsHandler : MonoBehaviour
{
    [SerializeField] private UMUBot owner;
    [SerializeField] private AnimatorEventsRepeater eventsRepeater;
    
    private Dictionary<string, Action> commandsDictionary;
    private void OnEnable()
    {
        FillCommandsDictionary();
        eventsRepeater.OnEventRecieved += ExecuteCommand;
    }

    private void FillCommandsDictionary()
    {
        commandsDictionary = new Dictionary<string, Action>();
        commandsDictionary.Add("Jump", owner.Jump);
    }
    
    private void ExecuteCommand(string obj)
    {
        commandsDictionary[obj].Invoke();
    }
}
