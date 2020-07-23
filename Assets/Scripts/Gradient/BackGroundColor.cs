using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundColor : MonoBehaviour
{
    public Color FromColor { get; private set; }
    public Color ToColor { get; private set; }

    private Color _currentFromColor;
    private Color _currentToColor;

    private Renderer _renderer = null;
    private Material _material = null;
    // Start is called before the first frame update
    void Start()
    {
        _currentFromColor = FromColor = GradientManager.Instance.GetFromColor();
        _currentToColor = ToColor = GradientManager.Instance.GetToColor();
        _renderer = GetComponentInChildren<Renderer>();
        _material = _renderer.material;
    }

    public void Apply(Color from, Color to)
    {
        FromColor = from;
        ToColor = to;
    }
    // Update is called once per frame
    void Update()
    {
        _currentFromColor = Color.Lerp(_currentFromColor, FromColor, Time.deltaTime);
        _currentToColor = Color.Lerp(_currentToColor, ToColor, Time.deltaTime);
        _material.SetColor("_SrcOneValue", _currentToColor);
        _material.SetColor("_SrcTwoValue", _currentFromColor);
        _renderer.material = _material;
    }

    public void ResetColor()
    {
        FromColor = GradientManager.Instance.GetFromColor();
        ToColor = GradientManager.Instance.GetToColor();
    }
}
