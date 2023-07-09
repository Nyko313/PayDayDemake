using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float magCapacity;
    [SerializeField] private bool isAutomatic;
    [SerializeField] private float reloadSpeed;
    [SerializeField] private AudioClip shootAudioClip;
    [SerializeField] private AudioClip reloadAudioClip;
    [SerializeField] private GameObject prefab;

    public float Damage { get => damage; private set { damage = value; } }
    public float FireRate { get => fireRate; private set { fireRate = value; } }
    public float MagCapacity { get => magCapacity; private set { magCapacity = value; } }
    public bool IsAutomatic { get => isAutomatic; private set { isAutomatic = value; } }
    public float ReloadSpeed { get => reloadSpeed; private set { reloadSpeed = value; } }
    public AudioClip ShootAudioClip { get => shootAudioClip; private set { shootAudioClip = value; } }
    public AudioClip ReloadAudioClip { get => reloadAudioClip; private set {  reloadAudioClip = value; } }
    public GameObject Prefab { get => prefab; private set { prefab = value; } }
}
