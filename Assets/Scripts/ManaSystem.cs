using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaSystem : MonoBehaviour
{
    public float maxMana = 100f;
    public float currentMana;
    public float manaRegenRate; //Mana per second
    public float regenDelay = 3f; //Segundos de espera antes de regenerar mana

    private float timeSinceLastManaUse = 0f;

    public ManaBar manaBar;

    void Start()
    {
        currentMana = maxMana;
        if (manaBar != null)
            manaBar.SetMaxMana(maxMana);
    }

    void Update()
    {
        timeSinceLastManaUse += Time.deltaTime;
        if (timeSinceLastManaUse >= regenDelay)
        {
            RegenerateMana();
        }
        UpdateManaUI();
    }

    public bool UseMana(float amount)
    {
        if(currentMana >= amount)
        {
            currentMana -= amount;
            timeSinceLastManaUse = 0f; //Reiniciar el temporizador de regeneracion mana
            UpdateManaUI();
            return true; //Mana suficiente
        }
        return false; //Mana insuficiente
    }

    void RegenerateMana()
    {
        if(currentMana < maxMana)
        {
            currentMana += manaRegenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana); //No exceder el maximo
        }
    }


    void UpdateManaUI()
    {
        if (manaBar != null)
        {

            manaBar.SetMana(currentMana);
        }
    }
}
