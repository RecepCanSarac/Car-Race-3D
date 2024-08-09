using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName =" Car",menuName ="Car/Car")]
public class SOPlayer : ScriptableObject
{
    public GameObject Car;
    public Vector3 Position;
    public Vector3 rotation;
}
