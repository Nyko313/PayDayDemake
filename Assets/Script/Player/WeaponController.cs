using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Niko313.PayDayDemake.Player
{
    public class WeaponController : MonoBehaviour
    {
        [Header("Equipped Weapons")]
        [SerializeField] private Weapon primaryWeapon;
        [SerializeField] private Weapon secondaryWeapon;

        [Header("Animation")]
        [SerializeField] private float weaponAnimationSpeed;
        [SerializeField] private Vector3 hidedWeaponRotation;
        [SerializeField] private float animationAccuracy;

        [Header("Others")]
        [SerializeField] private Transform weaponHandler;
        [SerializeField] private LayerMask shootableMask;
        [SerializeField] private bool canShoot;

        private GameObject weaponInstance;

        private RaycastHit shootHit;

        private bool triggerPressed;
        private bool isShooting;
        


        private float shootTimer;

        private Coroutine weaponAnimCoroutine;
        private bool weaponAnimInProgress = false;
        private Quaternion weaponInitialRotation;

        private EquippedWeapon currentWeapon;
        private EquippedWeapon primWeapon;
        private EquippedWeapon secWeapon;

        private WeaponState weaponState = WeaponState.None;

        private PlayerController player;

        private class EquippedWeapon
        {
            public Weapon weapon;
            public float ammoInMag;
            public float ammo;
        }

        private enum WeaponState
        {
            None,
            Equipping,
            Unequipping,
            Ready,
            Shooting,
            Reloading
        }

        #region Loop Methods
        public void Init(PlayerController player)
        {
            this.player = player;

            weaponInitialRotation = Quaternion.Euler(Vector3.zero);

            primWeapon = new EquippedWeapon();
            primWeapon.weapon = primaryWeapon;
            primWeapon.ammoInMag = primWeapon.weapon.MagCapacity;
            secWeapon = new EquippedWeapon();
            secWeapon.weapon = secondaryWeapon;
            secWeapon.ammoInMag = secWeapon.weapon.MagCapacity;

            EquipWeapon(primWeapon);
        }

        public void Update()
        {
            shootTimer += Time.deltaTime;

            if (triggerPressed)
            {
                if (canShoot && weaponState == WeaponState.Ready && (currentWeapon.weapon.IsAutomatic || isShooting == false) && currentWeapon.ammoInMag > 0 && shootTimer > currentWeapon.weapon.FireRate)
                {
                    Shoot();
                }
            }
            else
            {
                isShooting = false;
            }

        }
        #endregion

        #region Private

        private void EquipWeapon(EquippedWeapon weapon)
        {
            weaponState = WeaponState.Equipping;
            

            StartCoroutine(EquipWeaponCoroutine(weapon));
            
        }

        private IEnumerator EquipWeaponCoroutine(EquippedWeapon weapon)
        {

            if (weaponInstance != null)
            {
                weaponAnimCoroutine = StartCoroutine(HideWeaponCoroutine());
            }

            while (weaponAnimInProgress)
            {
                yield return null;
            }

            currentWeapon = weapon;

            if (weaponInstance != null)
            {
                Destroy(weaponInstance);
            }


            weaponInstance = Instantiate(currentWeapon.weapon.Prefab, weaponHandler.transform);
            

            weaponAnimCoroutine = StartCoroutine(ShowWeaponCoroutine());

            while (weaponAnimInProgress)
            {
                yield return null;
            }

            weaponState = WeaponState.Ready;
        }

        private void Shoot()
        {
            player.AudioSource.PlayOneShot(currentWeapon.weapon.ShootAudioClip);
            ShootRaycast(player.FPCameraLook.CameraHandler.transform.forward);

            shootTimer = 0f;
            
            isShooting = true;
            currentWeapon.ammoInMag -= 1;
        }
        private void StartReloading()
        {
            weaponState = WeaponState.Reloading;
            player.AudioSource.PlayOneShot(currentWeapon.weapon.ReloadAudioClip);
            StartCoroutine(ReloadingCoroutine());
        }

        private IEnumerator ReloadingCoroutine()
        {

            weaponAnimCoroutine = StartCoroutine(HideWeaponCoroutine());
            yield return new WaitForSeconds(currentWeapon.weapon.ReloadSpeed);
            weaponAnimCoroutine = StartCoroutine(ShowWeaponCoroutine());

            StopReloading();
        }

        private void StopReloading()
        {
            if (currentWeapon.ammo < currentWeapon.weapon.MagCapacity)
            {
                currentWeapon.ammo = 0;
            }
            else
            {
                currentWeapon.ammo -= currentWeapon.weapon.MagCapacity - currentWeapon.ammoInMag;
            }
            currentWeapon.ammoInMag = currentWeapon.weapon.MagCapacity;

            weaponState = WeaponState.Ready;
        }

        private bool ShootRaycast(Vector3 direction)
        {
            return Physics.Raycast(player.FPCameraLook.CameraHandler.transform.position, direction, out shootHit, 1000, shootableMask);
        }

        // TODO: Create Script for weapon Animations
        // Weapon Animation
        private IEnumerator ShowWeaponCoroutine()
        {
            weaponAnimInProgress = true;

            Quaternion targetRotation = Quaternion.Euler(hidedWeaponRotation);

            weaponHandler.localRotation = targetRotation;

            float elapsedTime = 0f;

            while (Quaternion.Angle(weaponHandler.localRotation, weaponInitialRotation) > animationAccuracy)
            {
                weaponHandler.localRotation = Quaternion.Lerp(targetRotation, weaponInitialRotation, weaponAnimationSpeed * elapsedTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            weaponAnimInProgress = false;

        }

        private IEnumerator HideWeaponCoroutine() 
        {
            weaponAnimInProgress = true;

            Quaternion targetRotation = Quaternion.Euler(hidedWeaponRotation);

            weaponHandler.localRotation = weaponInitialRotation;

            float elapsedTime = 0f;
            while (Quaternion.Angle(weaponHandler.localRotation, targetRotation) > animationAccuracy)
            {
                weaponHandler.localRotation = Quaternion.Lerp(weaponInitialRotation, targetRotation, weaponAnimationSpeed * elapsedTime);

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            weaponAnimInProgress = false;
        }
        #endregion

        #region Input
        private void OnShoot(InputValue value)
        {
            triggerPressed = value.isPressed && canShoot;

        }

        private void OnReload() 
        {
            if(weaponState == WeaponState.Ready)
            {
                StartReloading();
            }
        }

        private void OnSwitchWeapon()
        {
            if(currentWeapon.weapon == primWeapon.weapon)
            {
                EquipWeapon(secWeapon);
            }
            else
            {
                EquipWeapon(primWeapon);
            }
        }
        #endregion
    }
}

