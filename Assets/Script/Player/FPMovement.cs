using UnityEngine;
using UnityEngine.InputSystem;

public class FPMovement : MonoBehaviour
{
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

    public enum MovementState
    {
        Walking,
        Running,
        Crouching,
        Jumping
    }

    #region Unity Methods
    private void Start()
    {
        player = GetComponent<PlayerController>();
        characterController = player.CharacterController;

        // Setup Character controller
        characterController.center = new Vector3(0, player.StandHeight / 2, 0);
        characterController.height = player.StandHeight;
        camHeightOffset = player.StandHeight - player.CameraHandler.localPosition.y;

        // Get Camera Position
        Vector3 camPos = player.CameraHandler.transform.localPosition;
        camPos.y = characterController.height - camHeightOffset;
        player.CameraHandler.transform.localPosition = camPos;
    }
    private void Update()
    {

        // Check if the player is still jumping
        if (player.IsJumping && characterController.isGrounded)
        {
            player.IsJumping = false;
        }
    }

    private void FixedUpdate()
    {
        if (player.CanMove) {
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
        if(!characterController.isGrounded)
        {
            movementState = MovementState.Jumping;
        }else if (!player.IsCrouching && movementState != MovementState.Running)
        {
            movementState = MovementState.Walking;
        }
        
        // Check current target speed
        switch (movementState)
        {
            case MovementState.Walking:

                currentSpeed = player.WalkSpeed;
                currentForwardSpeed = player.WalkSpeed;
                break;
            case MovementState.Running:
                currentSpeed = player.WalkSpeed;
                currentForwardSpeed = input.y > 0 ? player.RunSpeed : player.WalkSpeed;
                break;
            case MovementState.Crouching:
                currentSpeed = player.CrouchSpeed;
                currentForwardSpeed = player.CrouchSpeed;
                break;
            case MovementState.Jumping:
                currentSpeed = player.JumpSpeed;
                currentForwardSpeed = player.JumpSpeed;
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
            momentum.x *= player.MovementFriction;
            momentum.z *= player.MovementFriction;
        }
        else
        {
            momentum.x *= player.AirFriction;
            momentum.z *= player.AirFriction;
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
            momentum += SlopeSlide() * player.SlopeFallSpeed;
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
        if (characterController.isGrounded && !player.IsJumping && player.CanMove)
        {
            player.IsJumping = true;
            momentum.y = player.JumpForce;
        }

    }

    /// <summary>
    /// Start Crouching
    /// </summary>
    private void Crouch()
    {
        if (player.IsCrouching == true && player.CanMove)
        {
            if (!Physics.Raycast(characterController.transform.position +  transform.up * characterController.height, transform.up, player.StandHeight - characterController.height, player.CrouchStandLayerMask))
            {
                player.IsCrouching = false;
            }
        }
        else
        {
            player.IsCrouching = true;
        }
    }

    /// <summary>
    /// Control the right height of the player
    /// </summary>
    private void Crouching()
    {
        float targetHeight = player.IsCrouching ? player.CrouchHeight : player.StandHeight;

        if (characterController.height != targetHeight)
        {
            AdjustHeight(targetHeight);

            Vector3 camPos = player.CameraHandler.transform.localPosition;
            camPos.y = characterController.height - camHeightOffset;
            player.CameraHandler.transform.localPosition = camPos;
        }

        if (player.IsCrouching)
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
        characterController.height = Mathf.Lerp(startHeight, height, player.CrouchAnimationSpeed);
        characterController.center = Vector3.Lerp(startCenter, new Vector3(0, center, 0), player.CrouchAnimationSpeed);
    }

    #endregion

    #region Input
    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if(value.isPressed)
        {
            Jump();
        }
        
    }

    public void OnCrouch(InputValue value)
    {
        if(value.isPressed)
        {
            Crouch();
        }
        
    }

    public void OnSprint(InputValue value)
    {
        if(movementState != MovementState.Jumping)
        {
            if (value.isPressed)
            {
                movementState = MovementState.Running;
                player.IsCrouching = false;
            }
            else
            {
                movementState = MovementState.Walking;
            }
        }

        
    }
    #endregion
}
