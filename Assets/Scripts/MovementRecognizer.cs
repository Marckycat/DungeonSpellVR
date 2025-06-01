using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using PDollarGestureRecognizer;
using System.IO;
using UnityEngine.Events;

public class MovementRecognizer : MonoBehaviour
{
    public XRNode inputSource;
    public InputHelpers.Button inputButton;
    public float inputThreshold = 0.1f;
    public InputHelpers.Button grabButton = InputHelpers.Button.Grip;
    public float grabThreshold = 0.1f;
    public Transform movementSource;

    public ObjectSpawner objectSpawner;

    public float newPositionThresholdDistance = 0.005f;
    public GameObject cubePrefab;
    public bool creationMode = true;
    public string newGestureName;

    public float recognitionThreshold = 0.9f;

    //// --- Variables para el disparo cargado ---
    //private bool isChargingShot = false;
    //private float chargeStartTime;
    //public float minChargeTimeToLaunch = 0.1f; // Tiempo mínimo para que se considere un lanzamiento (evitar toques accidentales)
    //public float minChargeTimeForBonus = 0.25f; // Tiempo mínimo de carga para empezar a obtener bonus de daño
    //public float maxChargeTime = 2.0f;         // Tiempo para alcanzar el multiplicador máximo
    //public float minDamageMultiplier = 1.0f;   // Multiplicador con carga mínima (o sin carga bonus)
    //public float maxDamageMultiplier = 3.0f;   // Multiplicador con carga máxima
    //// --- Fin variables para el disparo cargado ---

    [System.Serializable]
    public class UnityStringEvent : UnityEvent<string> { }
    public UnityStringEvent OnRecognized;

    private List<Gesture> trainingSet = new List<Gesture>();
    private bool isMoving = false;
    private List<Vector3> positionsList = new List<Vector3>();

    void Start()
    {
        string[] gestureFiles = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (var item in gestureFiles)
        {
            trainingSet.Add(GestureIO.ReadGestureFromFile(item));
        }
        if (objectSpawner != null)
        {
            objectSpawner.movementSource = movementSource; // Asigna la referencia para lanzar
        }
    }

    void Update()
    {
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), inputButton, out bool isPressed, inputThreshold);
        //Detectar boton para lanzar el objeto invocado
        InputHelpers.IsPressed(InputDevices.GetDeviceAtXRNode(inputSource), grabButton, out bool isGrabbed, grabThreshold);

        //Start The Movement
        if (!isMoving && isPressed)
        {
            StartMovement();
        }
        //Ending the Movement
        else if (isMoving && !isPressed)
        {
            EndMovement();
        }
        //Update the movement state
        else if (isMoving && isPressed)
        {
            UpdateMovement();
        }
        //Lanzar objeto al presionar GRAB
        if (isGrabbed && objectSpawner != null)
        {
            LaunchMovement();
        }
        //--Logica de disparo cargado--
        //if(isGrabbed && !isChargingShot && objectSpawner != null && objectSpawner.LaunchCurrentObject())
        //{
        //    StartCharge();
        //}
        //else if (!isGrabbed && isChargingShot)
        //{
        //    ReleaseChargeAndLaunch();
        //}
    }

    void StartMovement()
    {
        //Debug.Log("Movement Started");
        isMoving = true;
        positionsList.Clear();
        positionsList.Add(movementSource.position);
        if (cubePrefab)
        {
            Destroy(Instantiate(cubePrefab, movementSource.position, Quaternion.identity), 3);
        }
    }

    void EndMovement()
    {
        //Debug.Log("Movement Ended");
        isMoving = false;
        //if (positionsList.Count < 2) return;

        //Create the Gesture from the Position List
        Point[] pointArray = new Point[positionsList.Count];
        for (int i = 0; i < positionsList.Count; i++)
        {
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(positionsList[i]);
            pointArray[i] = new Point(screenPoint.x, screenPoint.y, 0);
        }

        Gesture newGesture = new Gesture(pointArray);
        //Add a new gesture to training set
        if (creationMode)
        {
            newGesture.Name = newGestureName;
            trainingSet.Add(newGesture);

            string fileName = Application.persistentDataPath + "/" + newGestureName + ".xml";
            GestureIO.WriteGesture(pointArray, newGestureName, fileName);
        }
        //recognize
        else
        {
            Result result = PointCloudRecognizer.Classify(newGesture, trainingSet.ToArray());
            Debug.Log(result.GestureClass + result.Score);
            if (result.Score > recognitionThreshold)
            {
                OnRecognized.Invoke(result.GestureClass);
            }
        }
    }

    void UpdateMovement()
    {
        //Debug.Log("Update Movement");
        Vector3 lastPosition = positionsList[positionsList.Count - 1];
        if(Vector3.Distance(movementSource.position, lastPosition) > newPositionThresholdDistance)
        {
            if (cubePrefab)
                Destroy(Instantiate(cubePrefab, movementSource.position, Quaternion.identity), 3);
            positionsList.Add(movementSource.position);
        }
            
    }

    void LaunchMovement()
    {
        objectSpawner.LaunchCurrentObject(10f);
    }
}
