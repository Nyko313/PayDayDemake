using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Rifle", menuName = "Weapons/Rifle")]
public class Rifle : Weapon
{
    [SerializeField] private bool automatic;
}
