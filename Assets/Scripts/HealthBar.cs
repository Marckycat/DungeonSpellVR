using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("UI")]
    public Gradient gradient;
    public Image healthImage;

    private float maxHealth;

    public void SetMaxHealth(float health)
    {
        maxHealth = health;
        SetHealth(health);
    }

    public void SetHealth(float health)
    {
        float normalizedHealth = Mathf.Clamp01(health / maxHealth);
        healthImage.fillAmount = normalizedHealth;
        healthImage.color = gradient.Evaluate(normalizedHealth);
    }
}
