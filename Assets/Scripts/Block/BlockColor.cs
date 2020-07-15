using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockColor : MonoBehaviour
{
    public Color Color { get; set; }
    public void setColor()
    {
        Color = GradientManager.Instance.GetColor();
    }

    public void applyColor()
    {
        var renderer = GetComponent<Renderer>();
        Material material = renderer.material;
        material.SetColor("_Color", Color);
        renderer.material = material;
    }
    
    public void applyColor(Color color)
    {
        Color = color;
        applyColor();
    }
}
