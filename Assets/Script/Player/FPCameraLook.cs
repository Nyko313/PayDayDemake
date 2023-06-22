using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Niko313.PayDayDemake.Player
{
    public class FPCameraLook : MonoBehaviour
    {

        [Title("Sensitivity")]
        [BoxGroup("Camera Look")][SerializeField] private float mouseSensitivity = 30f;
        [BoxGroup("Camera Look")][SerializeField] private float controllerSensitivity = 100f;

        [Title("Camera settings")]
        [BoxGroup("Camera Look")][SerializeField] private float maxClampAngle = 90;
        [BoxGroup("Camera Look")][SerializeField] private float minClampAngle = -90;
        [BoxGroup("Camera Look")][SerializeField] private bool canRotateCamera = true;
        [BoxGroup("Camera Look")][SerializeField] private LayerMask crouchStandLayerMask;

        [Title("Transforms")]
        [BoxGroup("Camera Look")][SerializeField] private Transform cameraHandler;
        [BoxGroup("Camera Look")][SerializeField] private Transform playerBody;

        [Title("Components")]
        [BoxGroup("Camera Look")][SerializeField] private Camera mainCamera;

        private Vector3 input;
        private float xRotation = 0f;

        private PlayerInput playerInput;
        private PlayerController player;

        #region Getters & Setters
        public float MouseSensitivity { get => mouseSensitivity; private set { mouseSensitivity = value; } }
        public float ControllerSensitivity { get => controllerSensitivity; private set { controllerSensitivity = value; } }

        public float MaxClampAngle { get => maxClampAngle; private set { maxClampAngle = value; } }
        public float MinClampAngle { get => minClampAngle; private set { minClampAngle = value; } }
        public bool CanRotateCamera { get => canRotateCamera; set { canRotateCamera = value; } }
        public LayerMask CrouchStandLayerMask { get => crouchStandLayerMask; private set { crouchStandLayerMask = value; } }

        public Transform CameraHandler { get => cameraHandler; private set { cameraHandler = value; } }
        public Transform PlayerBody { get => playerBody; private set { playerBody = value; } }

        public Camera MainCamera { get => mainCamera; private set { mainCamera = value; } }
        #endregion


        #region Loop Methods


        public void Init(PlayerController player)
        {
            this.player = player;


            HideCursor();
            xRotation = 0;
            CameraHandler.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        }

        public void UpdateComponent()
        {


            if (CanRotateCamera)
            {
                CameraRotation();
            }
        }
        #endregion

        #region Public

        public void DisableCameraRotation()
        {
            CanRotateCamera = false;

        }

        public void EnableCameraRotation()
        {
            CanRotateCamera = true;
        }

        public void ShowCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void HideCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        #endregion

        #region Private
        private void CameraRotation()
        {
            float mouseX = 0, mouseY = 0;

            var delta = input * 0.01f;
            mouseX += delta.x;
            mouseY += delta.y;

            switch (player.PlayerInput.currentControlScheme)
            {
                case "Keyboard&Mouse":
                    mouseX *= MouseSensitivity;
                    mouseY *= MouseSensitivity;
                    break;
                case "Gamepad":
                    mouseX *= ControllerSensitivity;
                    mouseY *= ControllerSensitivity;
                    break;
            }

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, MinClampAngle, MaxClampAngle);

            CameraHandler.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            PlayerBody.Rotate(Vector3.up * mouseX);
        }


        #endregion

        public void OnLook(InputValue value)
        {
            input = value.Get<Vector2>();
        }
    }
}


