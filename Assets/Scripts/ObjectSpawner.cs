using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    [System.Serializable]
    public class PooledObject
    {
        public string name;
        public GameObject prefab;
        public int initialCount = 3;
    }

    public List<PooledObject> objects;
    public Transform spawnOrigin;
    public Transform movementSource;

    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();
    private GameObject currentObject;

    public ManaSystem manaSystem;
    public float manaCostPerSpell = 20f; //Costo de cada invocacion

    private void Start()
    {
        // Crear pools
        foreach (var obj in objects)
        {
            if (!objectPool.ContainsKey(obj.name))
            {
                objectPool[obj.name] = new Queue<GameObject>();
                for (int i = 0; i < obj.initialCount; i++)
                {
                    GameObject newObj = Instantiate(obj.prefab);
                    newObj.name = obj.name;
                    newObj.SetActive(false);
                    objectPool[obj.name].Enqueue(newObj);
                }
            }
        }
    }

    public void Spawn(string objectName)
    {
        // Si ya hay uno activo, desactivarlo y devolverlo al pool
        if (currentObject != null)
        {
            ReturnToPool(currentObject);
            currentObject = null;
        }

        // Obtener del pool
        if (!objectPool.ContainsKey(objectName))
        {
            Debug.LogError("No existe un pool para: " + objectName);
            return;
        }

        GameObject objToSpawn = null;
        if (objectPool[objectName].Count > 0)
        {
            objToSpawn = objectPool[objectName].Dequeue();
        }
        else
        {
            // Instancia extra si el pool está vacío
            var prefab = objects.Find(p => p.name == objectName)?.prefab;
            if (prefab != null)
            {
                objToSpawn = Instantiate(prefab);
                objToSpawn.name = objectName;
            }
        }

        //Solo si objToSpawn no es null, se procede
        if (objToSpawn != null)
        {
            //Verifica si hay mana suficiente **antes de mostrar**
            if(manaSystem != null && !manaSystem.UseMana(manaCostPerSpell))
            {
                //Devuelve el pool si no hay mana suficiente
                objectPool[objectName].Enqueue(objToSpawn);
                objToSpawn.SetActive(false);
                Debug.Log("No hay mana suficiente para invocar: " + objectName);
                return;
            }

            objToSpawn.SetActive(true);
            objToSpawn.transform.SetParent(movementSource);
            objToSpawn.transform.localPosition = Vector3.zero;
            objToSpawn.transform.localRotation = Quaternion.identity;

            Rigidbody rb = objToSpawn.GetComponentInChildren<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            currentObject = objToSpawn;
        }
    }

    public void LaunchCurrentObject(float force)
    {
        if (currentObject != null)
        {
            Rigidbody rb = currentObject.GetComponentInChildren<Rigidbody>(); 
            //var rb = currentObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                currentObject.transform.SetParent(null); // Liberarlo de la mano
                rb.isKinematic = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                //Direccion con Raycast desde movementSource
                Vector3 launchDirection;
                Ray ray = new Ray(movementSource.position, movementSource.forward);
                RaycastHit hit;

                if(Physics.Raycast(ray, out hit, 100f))
                {
                    launchDirection = (hit.point - movementSource.transform.position).normalized;
                }
                else
                {
                    launchDirection = movementSource.forward;
                }
                rb.AddForce(movementSource.forward * force, ForceMode.Impulse);

                // Programar devolución al pool tras un tiempo
                StartCoroutine(DeactivateAfterTime(currentObject, 1.5f));
                currentObject = null;
            }
        }
    }
    private IEnumerator DeactivateAfterTime(GameObject obj, float time)
    {
        yield return new WaitForSeconds(time);
        ReturnToPool(obj);
    }

    private void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(null);
        if (objectPool.ContainsKey(obj.name))
        {
            objectPool[obj.name].Enqueue(obj);
        }
    }
}
