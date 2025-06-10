using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    [Header("Configuracion de Oleadas")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    public float timeBetweenWaves = 5f;
    public string winScene = "Win";

    [Header("UI Elements")]
    [SerializeField]
    private int currentWaveIndex = 0;
    private int enemiesRemaining;

    private int[] enemiesPerWave = new int[] { 2, 4, 8 };

    void Start()
    {
        StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        if(currentWaveIndex >= enemiesPerWave.Length)
        {
            Debug.Log("All waves completed!");
            yield break;
        }
        Debug.Log($"Iniciando Oleada {currentWaveIndex + 1}");
        yield return new WaitForSeconds(timeBetweenWaves);
        SpawnWave();
    }

    void SpawnWave()
    {
        enemiesRemaining = enemiesPerWave[currentWaveIndex];

        for (int i = 0; i < enemiesRemaining; i++)
        {
            Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject newEnemy = Instantiate(enemyPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);

            Enemie enemieScript = newEnemy.GetComponent<Enemie>();
            if (enemieScript != null)
            {
                enemieScript.waveManager = this;
            }
            else
            {
                Debug.LogError("El prefab del enemigo no tiene el script 'Enemie.cs'!");
            }
        }
    }


    public void OnEnemyDefeated()
    {
        enemiesRemaining--;
        Debug.Log($"Enemigo derrotado. Quedan: {enemiesRemaining}");

        if(enemiesRemaining <= 0)
        {
            Debug.Log($"Oleada {currentWaveIndex + 1} completada.");
            currentWaveIndex++;
            StartCoroutine(StartNextWave());
        }
    }
}
