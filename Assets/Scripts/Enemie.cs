using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemie : MonoBehaviour, IDamage
{
    public Transform jugadorVR;
    public PlayerHeartVR healthPlayer;

    public float health = 100f;
    public float distanciaDePersecucion;
    public float distanciaDeAtaque;

    public float damagePlayer; // Cantidad de daño que inflige al jugador al atacar
    public float timeBetweenAttacks; // Tiempo entre ataques al jugador
    private float nextAttackTime; // Controla el tiempo para el siguiente ataque

    private GameManager gameManager;
    private NavMeshAgent agente;

    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

        //Si no se asigno el jugador en el Inspector, intentara encontrarla
        if(jugadorVR == null)
        {
            GameObject jugadorObjeto = GameObject.FindGameObjectWithTag("Player");
            if (jugadorObjeto != null)
            {
                jugadorVR = jugadorObjeto.transform;
                healthPlayer = jugadorObjeto.GetComponent<PlayerHeartVR>();
                if(healthPlayer == null)
                {
                    Debug.LogError("No se encontró el componente PlayerHeartVR en el objeto del jugador. Asegúrate de que el jugador tenga este componente.");
                    enabled = false; // Deshabilita el script si no hay PlayerHeartVR
                }
            }
            else
            {
                Debug.LogError("No se encontró el objeto del jugador. Asegúrate de que tenga el Tag 'Player' o asígnalo manualmente.");
                enabled = false; // Deshabilita el script si no hay jugador
            }
        }
        else
        {
            //si el jugador fue asignado manualmente, obtenemos su script PlayerHeartVR
            healthPlayer = jugadorVR.GetComponent<PlayerHeartVR>();
            if(healthPlayer == null)
            {
                //Intenta buscar en los padres si la referencia no se asignó correctamente
                healthPlayer = jugadorVR.GetComponentInParent<PlayerHeartVR>();
                if (healthPlayer == null)
                {
                    Debug.LogError("No se encontró el componente PlayerHeartVR en el objeto del jugador. Asegúrate de que el jugador tenga este componente.");
                    enabled = false; // Deshabilita el script si no hay PlayerHeartVR
                }
            }
        }
        // Configurar la distancia de parada del agente para que coincida con la distancia de ataque
        if (agente != null)
        {
            agente.stoppingDistance = distanciaDeAtaque;
        }

        // Encontrar el GameManager en la escena
        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager no encontrado en la escena.");
        }
    }

    void Update()
    {
        if(jugadorVR == null || agente == null || healthPlayer == null || !healthPlayer.gameObject.activeInHierarchy)
        {
            //Si el jugador no existe, esta muerto (desactivado), o no hay agente, no hacer nada
            if (agente != null && agente.isOnNavMesh) agente.isStopped = true;
            return; 
        }

        // Calcula la distancia al jugador
        float distanciaAlJugador = Vector3.Distance(transform.position, jugadorVR.position);

        // Si el jugador está dentro de la distancia de persecución
        if (distanciaAlJugador <= distanciaDePersecucion)
        {
            agente.isStopped = false; // Asegúrate de que el agente no esté detenido
            // Establece el destino del agente NavMesh a la posición del jugador
            agente.SetDestination(jugadorVR.position);

            // Opcional: Hacer que el enemigo mire al jugador mientras persigue
            Vector3 direccionAlJugador = (jugadorVR.position - transform.position).normalized;
            if (direccionAlJugador != Vector3.zero)
            {
                Quaternion rotacionDeseada = Quaternion.LookRotation(new Vector3(direccionAlJugador.x, 0, direccionAlJugador.z)); // Ignora la diferencia de altura para la rotación
                transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, Time.deltaTime * agente.angularSpeed);
            }

            // Lógica adicional si está dentro de la distancia de ataque
            if (distanciaAlJugador <= agente.stoppingDistance) // Usa el stoppingDistance del agente
            {
                //Mirar directamente al jugador cuando esta en rango de ataque y parado
                Vector3 lookPos = jugadorVR.position - transform.position;
                lookPos.y = 0; //Mantener al enemigo mirando horizontalmente
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 10f);

                //Attack Player
                AttackPlayer();
            }
        }
        else
        {
            if (agente.isOnNavMesh)
            {
                agente.isStopped = true; // Detiene el agente si el jugador está fuera de la distancia de persecución
            }
        }
    }

    void AttackPlayer()
    {
        if(Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + timeBetweenAttacks; // Actualiza el tiempo para el próximo ataque

            //Aqui llamamos al metodo de daño del jugador
            if(healthPlayer != null)
            {
                healthPlayer.DamageAmmount(damagePlayer);
                Debug.Log($"{gameObject.name} ataca al jugador y le inflige {damagePlayer} de daño.");
            }
        }
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        Debug.Log($"{gameObject.name} recibio {amount} de daño. Vida restante: {health}");
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
        if (agente != null) agente.enabled = false; // Desactiva el agente NavMesh para que no siga moviéndose
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false; //Desactiva el collider

        // Notificar al GameManager
        if (gameManager != null)
        {
            gameManager.EnemigoEliminado();
        }
        else
        {
            Debug.LogWarning("Intento de notificar muerte de enemigo, pero no se encontró GameManager.");
        }
        Destroy(gameObject);
    }

    // Opcional: Dibuja gizmos en el editor para visualizar las distancias
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDePersecucion);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeAtaque);
    }
}
