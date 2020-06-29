using System;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable] 
public class SaveData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public SaveData(Transform data)
    {
        Position = data.position;
        Rotation = data.rotation;
    }
}
