using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Niko313.PayDayDemake.Player
{
    public class FPMovement : MonoBehaviour
    {
        [Title("Speeds")]
        [BoxGroup("Movements")][SerializeField] private float walkSpeed;
        [BoxGroup("Movements")][SerializeField] private float runSpeed;
        [BoxGroup("Movements")][SerializeField] private float crouchSpeed;
        [BoxGroup("Movements")][SerializeField] private float jumpSpeed;
        [BoxGroup("Movements")][SerializeField] private float movementAccelleration = 0.9f;

        [Title("Jump")]
        [BoxGroup("Movements")][SerializeField] private float jumpForce;

        [Title("Friction")]
        [BoxGroup("Movements")][MaxValue(1f)][SerializeField] private float movementFriction;
        [BoxGroup("Movements")][MaxValue(1f)][SerializeField] private float airFriction;

        [Title("Crouching")]
        [BoxGroup("Movements")][SerializeField] private float crouchAnimationSpeed;
        [BoxGroup("Movements")][SerializeField] private float crouchHeight = 1;
        [BoxGroup("Movements")][SerializeField] private float standHeight;

        [Title("Others")]
        [BoxGroup("Movements")][SerializeField] private float slopeFallSpeed;
        [BoxGroup("Movements")][SerializeField] private bool canMove = true;

        [Title("Debug")]
        [BoxGroup("Movements")][ReadOnly] private bool isJumping;
        [BoxGroup("Movements")][ReadOnly] private bool isCrouching;

        private const float Gravity = 9.8f;

        private Vector2 input;

        private float currentSpeed;
        private float currentForwardSpeed;

        private float camHeightOffset;

        private Vector3 momentum;
        private bool isWallSliding;

        private RaycastHit slopeHit;
        private MovementState movementState;

        private PlayerController player;
        private CharacterController characterController;

        #region Getters & Setters
        public float WalkSpeed { get => walkSpeed; private set { walkSpeed = value; } }
        public float RunSpeed { get => runSpeed; private set { runSpeed = value; } }
        public float CrouchSpeed { get => crouchSpeed; private set { crouchSpeed = value; } }
        public float JumpSpeed { get => jumpSpeed; private set { jumpSpeed = value; } }
        public float MovementAccelleration { get => movementAccelleration; private set { movementAccelleration = value; } }

        public float JumpForce { get => jumpForce; private set { jumpForce = value; } }

        public float MovementFriction { get => movementFriction; private set { movementFriction = value; } }
        public float AirFriction { get => airFriction; private set { airFriction = value; } }

        public float CrouchAnimationSpeed { get => crouchAnimationSpeed; private set { crouchAnimationSpeed = value; } }
        public float CrouchHeight { get => crouchHeight; private set { crouchHeight = value; } }
        public float StandHeight { get => standHeight; private set { standHeight = value; } }
        public float SlopeFallSpeed { get => slopeFallSpeed; private set { slopeFallSpeed = value; } }
        public bool CanMove { get => canMove; private set { canMove = value; } }

        public bool IsJumping { get => isJumping; set { isJumping = value; } }
        public bool IsCrouching { get => isCrouching; set { isCrouching = value; } }
        #endregion

        public enum MovementState
        {
            Walking,
            Running,
            Crouching,
            Jumping
        }

        #region Unity Methods
        public void Init(PlayerController player)
        {
            this.player = player;
            characterController = player.CharacterController;

            // Setup Character controller
            characterController.center = new Vector3(0, StandHeight / 2, 0);
            characterController.height = StandHeight;
            camHeightOffset = StandHeight - player.FPCameraLook.CameraHandler.localPosition.y;

            // Get Camera Position
            Vector3 camPos = player.FPCameraLook.CameraHandler.transform.localPosition;
            camPos.y = characterController.height - camHeightOffset;
            player.FPCameraLook.CameraHandler.transform.localPosition = camPos;
        }
        public void Update()
        {

            // Check if the player is still jumping
            if (IsJumping && characterController.isGrounded)
            {
                IsJumping = false;
            }
        }

        public void FixedUpdate()
        {
            if (CanMove)
            {
                Movement();
            }

            ApplyGravity();
            Crouching();
            IsWallSliding();

        }

        #endregion


        #region Private

        private void Movement()
        {
            MovementSpeed();
            MovementDirection();
            FinalMovement();
        }

        /// <summary>
        /// Get the right speed
        /// </summary>
        private void MovementSpeed()
        {
            if (!characterController.isGrounded)
            {
                movementState = MovementState.Jumping;
            }
            else if (!IsCrouching && movementState != MovementState.Running)
            {
                movementState = MovementState.Walking;
            }

            // Check current target speed
            switch (movementState)
            {
                case MovementState.Walking:

                    currentSpeed = WalkSpeed;
                    currentForwardSpeed = WalkSpeed;
                    break;
                case MovementState.Running:
                    currentSpeed = WalkSpeed;
                    currentForwardSpeed = input.y > 0 ? RunSpeed : WalkSpeed;
                    break;
                case MovementState.Crouching:
                    currentSpeed = CrouchSpeed;
                    currentForwardSpeed = CrouchSpeed;
                    break;
                case MovementState.Jumping:
                    currentSpeed = JumpSpeed;
                    currentForwardSpeed = JumpSpeed;
                    break;
            }
        }


        /// <summary>
        /// Get the movement direction
        /// </summary>
        private void MovementDirection()
        {
            // Apply friction to the player
            if (characterController.isGrounded)
            {
                momentum.x *= MovementFriction;
                momentum.z *= MovementFriction;
            }
            else
            {
                momentum.x *= AirFriction;
                momentum.z *= AirFriction;
            }

            momentum += transform.forward * input.y * currentForwardSpeed + transform.right * input.x * currentSpeed;

            //Change direction on slope
            if (OnSlope())
            {
                if (isWallSliding)
                {
                    momentum.y = -Mathf.Abs(momentum.y);
                }
                // NOT WORKING WITH MOMENTUM 
                //else
                //{
                //    momentum = Vector3.ProjectOnPlane(momentum, slopeHit.normal);
                //}
            }
        }

        /// <summary>
        /// Applying the final movement to the character controller
        /// </summary>
        private void FinalMovement()
        {

            if (isWallSliding)
            {
                momentum += SlopeSlide() * SlopeFallSpeed;
            }

            characterController.Move(momentum * Time.fixedDeltaTime);

        }

        // Apply gravity to the vertical velocity
        private void ApplyGravity()
        {
            if (!characterController.isGrounded || isWallSliding)
            {
                momentum.y -= Gravity * Time.fixedDeltaTime;
            }
            else if (movementState != MovementState.Jumping && !isWallSliding)
            {

                momentum.y = -1.5f;
            }
        }

        // Check if the player is on slope
        private bool OnSlope()
        {
            if (characterController.isGrounded)
            {
                if (SlopeRay())
                {
                    return slopeHit.normal != Vector3.up;
                }
            }
            return false;
        }

        private void IsWallSliding()
        {
            isWallSliding = characterController.isGrounded && SlopeRay() && Vector3.Angle(slopeHit.normal, Vector3.up) > characterController.slopeLimit;

        }


        private bool SlopeRay()
        {
            return Physics.Raycast(transform.position, -transform.up, out slopeHit);
        }

        // Get the slope direction
        private Vector3 SlopeSlide()
        {
            Vector3 groundNormalCross = Vector3.Cross(slopeHit.normal, Vector3.up);
            return -Vector3.Cross(groundNormalCross, slopeHit.normal);
        }

        /// <summary>
        /// Jump
        /// </summary>
        private void Jump()
        {
            if (characterController.isGrounded && !IsJumping && CanMove)
            {
                IsJumping = true;
                momentum.y = JumpForce;
            }

        }

        /// <summary>
        /// Start Crouching
        /// </summary>
        private void Crouch()
        {
            if (IsCrouching == true && CanMove)
            {
                if (!Physics.Raycast(characterController.transform.position + transform.up * characterController.height, transform.up, StandHeight - characterController.height, player.FPCameraLook.CrouchStandLayerMask))
                {
                    IsCrouching = false;
                }
            }
            else
            {
                IsCrouching = true;
            }
        }

        /// <summary>
        /// Control the right height of the player
        /// </summary>
        private void Crouching()
        {
            float targetHeight = IsCrouching ? CrouchHeight : StandHeight;

            if (characterController.height != targetHeight)
            {
                AdjustHeight(targetHeight);

                Vector3 camPos = player.FPCameraLook.CameraHandler.transform.localPosition;
                camPos.y = characterController.height - camHeightOffset;
                player.FPCameraLook.CameraHandler.transform.localPosition = camPos;
            }

            if (IsCrouching)
            {
                movementState = MovementState.Crouching;
            }
        }

        /// <summary>
        /// Adjust the height of the character controller
        /// </summary>
        /// <param name="height"></param>
        private void AdjustHeight(float height)
        {
            float center = height / 2;

            float startHeight = characterController.height;
            Vector3 startCenter = characterController.center;
            characterController.height = Mathf.Lerp(startHeight, height, CrouchAnimationSpeed);
            characterController.center = Vector3.Lerp(startCenter, new Vector3(0, center, 0), CrouchAnimationSpeed);
        }

        #endregion

        #region Input
        public void OnMove(InputValue value)
        {
            input = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            if (value.isPressed)
            {
                Jump();
            }

        }

        public void OnCrouch(InputValue value)
        {
            if (value.isPressed)
            {
                Crouch();
            }

        }

        public void OnSprint(InputValue value)
        {
            if (movementState != MovementState.Jumping)
            {
                if (value.isPressed)
                {
                    movementState = MovementState.Running;
                    IsCrouching = false;
                }
                else
                {
                    movementState = MovementState.Walking;
                }
            }


        }
        #endregion
    }

}

