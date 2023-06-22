using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Niko313.PayDayDemake.Player
{
    public class PlayerController : MonoBehaviour
    {
        public FPMovement FPMovement { get; private set; }
        public FPCameraLook FPCameraLook { get; private set; }
        public CharacterController CharacterController { get; private set; }
        public PlayerInput PlayerInput { get; private set; }
        public WeaponController WeaponController { get; private set; }


        private void Awake()
        {
            SetupComponents();
            InitializeComponents();
        }

        private void Start()
        {
            InitializeComponents();
        }

        private void Update()
        {
            UpdateComponents();
        }

        private void FixedUpdate()
        {
            FixedUpdateComponents();
        }

        private void SetupComponents()
        {
            PlayerInput = gameObject.GetComponent<PlayerInput>();
            CharacterController = GetComponent<CharacterController>();
            FPMovement = GetComponent<FPMovement>();
            FPCameraLook = GetComponent<FPCameraLook>();
            WeaponController = GetComponent<WeaponController>();
        }

        private void InitializeComponents()
        {
            FPMovement.Init(this);
            FPCameraLook.Init(this);
            WeaponController.Init(this);

        }

        private void UpdateComponents()
        {
            FPMovement.UpdateComponent();
            FPCameraLook.UpdateComponent();
        }

        private void FixedUpdateComponents()
        {
            FPMovement.FUpdateComponent();
        }
    }

}

