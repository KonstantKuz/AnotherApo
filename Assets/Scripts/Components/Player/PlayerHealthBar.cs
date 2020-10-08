using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthBar : SceneLineProgressBar<PlayerHealthBar>
{
    public override void OnEnable()
    {
        base.OnEnable();
        
        InitialData<PlayerHealthBar> initialData;
        initialData.MinValue = 0;
        initialData.MaxValue = Constants.TotalHealth.Player;
        initialData.CurrentValue = Constants.TotalHealth.Player;
        InitializeProgress(initialData);
        
        PlayerHealth.OnPlayerDamaged += delegate(int health)
        {
            UpdateData<PlayerHealthBar> updateData;
            updateData.CurrentValue = health;
            UpdateCurrentProgress(updateData);
        };
    }
}
