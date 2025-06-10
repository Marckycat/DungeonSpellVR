using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR;

public class SprintVR : MonoBehaviour
{
    public CharacterController characterController;
    public ActionBasedController lefthHandController;
    public ActionBasedController rightHandController;
    public Camera mainCamera;

    public float speed = 3.0f;
    public float velocityThreshold = 1.2f;

    public InputHelpers.Button grabButton = InputHelpers.Button.Grip;
    public float activationThreshold = 0.9f;

    void Update()
    {
        float leftGrip = lefthHandController.selectActionValue.action.ReadValue<float>();
        float rightGrip = rightHandController.selectActionValue.action.ReadValue<float>();

        bool leftGripPressed = leftGrip > 0.9f;
        bool rightGripPressed = rightGrip > 0.9f;

        if (leftGripPressed && rightGripPressed)
        {
            Vector3 leftVelocity = Vector3.zero;
            Vector3 rightVelocity = Vector3.zero;
            Debug.Log($"LeftVelocity: {leftVelocity.magnitude}, RightVelocity: {rightVelocity.magnitude}");



            if (leftVelocity.magnitude > velocityThreshold || rightVelocity.magnitude > velocityThreshold)
            {
                Debug.Log("Moviendo jugador...");
                MovePlayer();
            }
        }
    }

    void MovePlayer()
    {
        Vector3 moveDirection = mainCamera.transform.forward;
        moveDirection.y = 0;
        moveDirection.Normalize();

        characterController.Move(moveDirection * speed * Time.deltaTime);
    }
}
