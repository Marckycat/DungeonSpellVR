using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHeartVR : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;

    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        
    }

    public void DamageAmmount(float ammount)
    {
        currentHealth -= ammount;
        Debug.Log($"Jugador recibido {ammount} de daño. Salud restante: {currentHealth}");

        healthBar.SetHealth(currentHealth);

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Jugador muerto. Game Over");


        Time.timeScale = 0; //Detiene el juego
        SceneManager.LoadScene("GameOver");
    }
}
