using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
    [Header("UI")]
    public Gradient gradient;
    public Image manaImage;

    private float maxMana;

    public void SetMaxMana(float mana)
    {
        maxMana = mana;
        SetMana(mana);
    }

    public void SetMana(float mana)
    {
        float normalizedMana = Mathf.Clamp01(mana / maxMana);
        manaImage.color = gradient.Evaluate(normalizedMana);
    }
}
