using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockColor : MonoBehaviour
{

    public void setColor()
    {
        var renderer = GetComponent<Renderer>();
        Material material = renderer.material;
        material.SetColor("_Color", Color.red);
        renderer.material = material;
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
