using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealthViewer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText;

    public void OnEnable()
    {
        PlayerHealth.OnPlayerDamaged += SetText;
    }

    private void SetText(int currentHealth)
    {
        if (currentHealth <= 0)
        {
            healthText.SetText("ну ты кароче здох но пока тебе павезло живи");
            return;
        }

        healthText.SetText(currentHealth.ToString());
    }
}
