using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Animator _animator = null;
    [SerializeField] private ClickDetect _tapDetect = null;
    public event Action EventGoGame;
    
    void Awake()
    {
        _tapDetect.EventStartTapClick += OnStartTapClick;
        GameController.Instance.EventReStart += OnReStart;
    }

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void Destroy()
    {
        _tapDetect.EventStartTapClick -= OnStartTapClick;
    }
    
    private void OnStartTapClick()
    {
        _animator.SetTrigger("End");
        _tapDetect.EventStartTapClick -= OnStartTapClick;
        EventGoGame?.Invoke();
    }

    private void OnReStart()
    {
        _tapDetect.EventStartTapClick += OnStartTapClick;
        _animator.SetTrigger("Start");
    }
}
