using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private GameObject tipsNStart;
    public static Action OnGameStarted;
    public static bool IsGameStarted;
    
    public void StartGame()
    {
        tipsNStart.SetActive(false);
        IsGameStarted = true;
        OnGameStarted();
    }
}
