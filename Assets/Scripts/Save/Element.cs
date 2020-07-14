using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] 
public class Element
{
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Scale;
    //public Color Color;
    //public Gradient Gradient;

    public Element(Transform t)
    {
        Position = t.position;
        Rotation = t.rotation;
        Scale = t.localScale;
    }
}
