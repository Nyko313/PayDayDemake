using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Niko313.PayDay2Demake.Player
{
    public class FPCameraLook : MonoBehaviour
    {

        private Vector3 input;
        private float xRotation = 0f;

        private PlayerInput playerInput;
        private PlayerController player;


        #region Unity Methods

        private void Awake()
        {

        }

        void Start()
        {
            player = GetComponent<PlayerController>();
            

            HideCursor();
            xRotation = 0;
            player.CameraHandler.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

        }

        void Update()
        {


            if (player.CanRotateCamera)
            {
                CameraRotation();
            }
        }
        #endregion

        #region Public

        public void DisableCameraRotation()
        {
            player.CanRotateCamera = false;

        }

        public void EnableCameraRotation()
        {
            player.CanRotateCamera = true;
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
                    mouseX *= player.MouseSensitivity;
                    mouseY *= player.MouseSensitivity;
                    break;
                case "Gamepad":
                    mouseX *= player.ControllerSensitivity;
                    mouseY *= player.ControllerSensitivity;
                    break;
            }

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, player.MinClampAngle, player.MaxClampAngle);

            player.CameraHandler.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            player.PlayerBody.Rotate(Vector3.up * mouseX);
        }


        #endregion

        public void OnLook(InputValue value)
        {
            input = value.Get<Vector2>();
        }
    }
}


