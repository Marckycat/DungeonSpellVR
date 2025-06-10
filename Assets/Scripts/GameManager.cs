using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Asignar los enemigos 
    public List<GameObject> enemies = new List<GameObject>();

    public string winScene = "Win";

    private int numberOfEnemiesLifes;

    public EnemyManager waveManager;

    void Start()
    {
        numberOfEnemiesLifes = enemies.Count;

        // Alternativamente, si los enemigos se instancian dinámicamente,
        // podrías tener un método para registrarlos al ser creados.
        // O podrías encontrarlos por Tag:
        // GameObject[] enemigosEncontrados = GameObject.FindGameObjectsWithTag("Enemigo");
        // numberOfEnemiesLifes = enemigosEncontrados.Length;
        // foreach (GameObject enemigo in enemigosEncontrados)
        // {
        //     // Opcional: añadir una referencia al script del enemigo para detectar su muerte
        // }
    }

    //void Update()
    //{
        
    //}

    // Este método debe ser llamado por cada enemigo cuando es eliminado.
    public void EnemigoEliminado()
    {
        numberOfEnemiesLifes--;

        Debug.Log("Enemigo eliminado. Enemigos restantes: " + numberOfEnemiesLifes);

        // Comprobar si todos los enemigos han sido eliminados
        if (numberOfEnemiesLifes <= 0)
        {
            WinScene();
        }
    }

    void WinScene()
    {
        Debug.Log("¡Todos los enemigos eliminados! Cargando escena de ganar...");
        // Cargar la escena de ganar
        SceneManager.LoadScene(winScene);
    }

    // Opcional: Método para registrar enemigos si se instancian dinámicamente
    //public void RegistrarEnemigo(GameObject enemigo)
    //{
    //    if (!enemigos.Contains(enemigo))
    //    {
    //        enemigos.Add(enemigo);
    //    }
    //    ActualizarConteoDeEnemigosVivos();
    //}

    //// Opcional: Método para desregistrar enemigos si se destruyen y no llaman a EnemigoEliminado()
    //public void DesregistrarEnemigo(GameObject enemigo)
    //{
    //    if (enemigos.Contains(enemigo))
    //    {
    //        enemigos.Remove(enemigo);
    //    }
    //    ActualizarConteoDeEnemigosVivos(); // Esto es por si acaso, lo ideal es llamar a EnemigoEliminado
    //}

    private void ActualizarConteoDeEnemigosVivos()
    {
        numberOfEnemiesLifes = enemies.Count;
    }
}
