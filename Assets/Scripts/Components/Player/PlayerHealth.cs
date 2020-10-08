using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoCached, IDamageable
{
    public int TotalHealth { get; private set; }
    public static Action<int> OnPlayerDamaged;
    
    private void Start()
    {
        TotalHealth = Constants.TotalHealth.Player;
    }
    
    public void TakeDamage(int value)
    {
        TotalHealth -= value;
        OnPlayerDamaged?.Invoke(TotalHealth);
    }
}
