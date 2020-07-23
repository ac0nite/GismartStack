using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundColor : MonoBehaviour
{
    public Color FromColor { get; set; }
    public Color ToColor { get; set; }

    private Renderer _renderer = null;
    // Start is called before the first frame update
    void Start()
    {
        FromColor = GradientManager.Instance.GetFromColor();
        ToColor = GradientManager.Instance.GetToColor();
        _renderer = GetComponentInChildren<Renderer>();
        Material material = _renderer.material;
        material.SetColor("_SrcOneValue", ToColor);
        material.SetColor("_SrcTwoValue", FromColor);
        _renderer.material = material;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
