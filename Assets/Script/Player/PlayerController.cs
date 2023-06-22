using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
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
    [BoxGroup("Movements")][ReadOnly] private bool isGrounded;

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


    public FPMovement FPMovement { get; private set; }
    public CharacterController CharacterController { get; private set; }
    public PlayerInput PlayerInput { get; private set; }

    public float WalkSpeed { get => walkSpeed; private set { walkSpeed = value; } }
    public float RunSpeed { get => runSpeed; private set { runSpeed = value; } }
    public float CrouchSpeed { get => crouchSpeed; private set { crouchSpeed = value; } }
    public float JumpSpeed { get => jumpSpeed; private set { jumpSpeed = value; } }
    public float MovementAccelleration { get => movementAccelleration; private set { movementAccelleration= value; } }

    public float JumpForce { get => jumpForce; private set { jumpForce = value; } }

    public float MovementFriction { get => movementFriction; private set { movementFriction= value; } }
    public float AirFriction { get => airFriction; private set { airFriction= value; } }

    public float CrouchAnimationSpeed { get => crouchAnimationSpeed; private set { crouchAnimationSpeed = value; } }
    public float CrouchHeight { get => crouchHeight; private set { crouchHeight= value; } }
    public float StandHeight { get => standHeight; private set { standHeight = value; } }
    public float SlopeFallSpeed { get => slopeFallSpeed; private set { slopeFallSpeed = value; } }
    public bool CanMove { get => canMove; private set { canMove = value; } }

    public bool IsJumping { get => isJumping;  set { isJumping= value; } }
    public bool IsCrouching { get => isCrouching;  set { isCrouching = value; } }

    public float MouseSensitivity { get => mouseSensitivity; private set { mouseSensitivity = value; } }
    public float ControllerSensitivity { get => controllerSensitivity; private set { controllerSensitivity = value; } }

    public float MaxClampAngle { get => maxClampAngle; private set { maxClampAngle = value; } }
    public float MinClampAngle { get => minClampAngle; private set { minClampAngle = value; } }
    public bool CanRotateCamera { get => canRotateCamera;  set { canRotateCamera= value; } }
    public LayerMask CrouchStandLayerMask { get => crouchStandLayerMask; private set { crouchStandLayerMask = value; } }

    public Transform CameraHandler { get => cameraHandler; private set { cameraHandler= value; } }
    public Transform PlayerBody { get => playerBody; private set { playerBody = value; } }

    public Camera MainCamera { get => mainCamera; private set { mainCamera = value; } }


    public void Awake()
    {
        SetupComponents();
    }

    private void SetupComponents()
    {
        PlayerInput = gameObject.GetComponent<PlayerInput>();
        CharacterController = GetComponent<CharacterController>();
        FPMovement = GetComponent<FPMovement>();
    }
}
