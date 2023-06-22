using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Niko313.PayDayDemake.Player
{
    public class WeaponController : MonoBehaviour
    {
        [SerializeField] private Weapon currentWeapon;
        [SerializeField] private Transform weaponHandler;
        [SerializeField] private LayerMask shootableMask;
        [SerializeField] private bool canShoot;


        private RaycastHit shootHit;


        private PlayerController player;
        #region Loop Methods
        public void Init(PlayerController player)
        {
            this.player = player;
        }
        #endregion

        #region Private
        private void Shoot()
        {
            if(currentWeapon.Ammo > 0)
            {
                ShootRaycast(player.FPCameraLook.CameraHandler.transform.forward);
                Debug.Log(shootHit);
            }
        }

        private bool ShootRaycast(Vector3 direction)
        {
            return Physics.Raycast(player.FPCameraLook.CameraHandler.transform.position, direction, out shootHit, 1000, shootableMask);
        }
        #endregion

        #region Input
        private void OnShoot()
        {
            if(canShoot)
            {
                Shoot();
            }
        }
        #endregion
    }
}

