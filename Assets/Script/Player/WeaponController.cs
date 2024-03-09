using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Niko313.PayDayDemake.Player
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Weapon currentWeapon;
        [SerializeField] private Transform weaponHandler;
        [SerializeField] private LayerMask shootableMask;
        [SerializeField] private bool canShoot;


        private RaycastHit shootHit;

        private bool triggerPressed;

        private float shootTimer;


        private PlayerController player;
        #region Loop Methods
        public void Init(PlayerController player)
        {
            this.player = player;
        }

        public void UpdateComponent()
        {
            shootTimer += Time.deltaTime;
        }
        #endregion

        #region Private
        private void Shoot()
        {
            if(currentWeapon.Ammo > 0 && shootTimer > currentWeapon.FireRate)
            {
                ShootRaycast(player.FPCameraLook.CameraHandler.transform.forward);
            }
        }

        private bool ShootRaycast(Vector3 direction)
        {
            return Physics.Raycast(player.FPCameraLook.CameraHandler.transform.position, direction, out shootHit, 1000, shootableMask);
        }
        #endregion

        #region Input
        private void OnShoot(InputValue value)
        {
            if (value.isPressed && canShoot)
            {
                if (currentWeapon.IsAutomatic || triggerPressed == false )
                {
                    triggerPressed = true;
                    Shoot();
                }
            }
            else
            {
                triggerPressed = false;
            }
        }
        #endregion
    }
}

