using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private int ammo;
    [SerializeField] private float magCapacity;
    [SerializeField] private bool isAutomatic;
    [SerializeField] private AudioClip shootAudioClip;
    [SerializeField] private AudioClip reloadAudioClip;
    [SerializeField] private GameObject model;

    public float Damage { get => damage; private set { damage = value; } }
    public float FireRate { get => fireRate; private set { fireRate = value; } }
    public int Ammo { get => ammo; private set { ammo = value; } }
    public float MagCapacity { get => magCapacity; private set { magCapacity = value; } }
    public bool IsAutomatic { get => isAutomatic; private set { isAutomatic = value; } }
    public GameObject Model { get => model; private set{model = value;}}
}
