using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ClickDetect _tapDetect = null;
    public event Action EventGoGame;
    
    void Awake()
    {
        _tapDetect.EventEndScreen += OnEndScreen;
    }

    void Start()
    {
    }

    private void OnDestroy()
    {
        _tapDetect.EventEndScreen -= OnEndScreen;
    }

    private void OnEndScreen()
    {
        EventGoGame?.Invoke();
    }
}
