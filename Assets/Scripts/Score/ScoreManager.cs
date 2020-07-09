using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private int _scoreValue = 0;
    private int _recordValue = 0;
    [SerializeField] private Text _score = null;
    [SerializeField] private Text _record = null;
    [SerializeField] private UIManager _uiManager = null;
    
    private void Awake()
    {
        _score.text = _scoreValue.ToString();
        _recordValue = PlayerPrefs.GetInt("Record", 0);
        _record.text = _recordValue.ToString();
        GameController.Instance.EventChangeScore += OnChangeScore;
        GameController.Instance.EventChangeRecord += OnChangeRecord;
        _uiManager.EventGoGame += OnGoGame;
    }

    private void OnDestroy()
    {
        GameController.Instance.EventChangeScore -= OnChangeScore;
        GameController.Instance.EventChangeRecord -= OnChangeRecord;
        _uiManager.EventGoGame -= OnGoGame;
    }

    private void OnChangeScore()
    {
        _scoreValue++;
        _score.text = _scoreValue.ToString();
    }

    private void OnChangeRecord()
    {
        _recordValue = Mathf.Clamp(_scoreValue, _recordValue, Int32.MaxValue);
        _record.text = _recordValue.ToString();
        PlayerPrefs.SetInt("Record", _recordValue);
        PlayerPrefs.Save();
    }

    private void OnGoGame()
    {
        _scoreValue = 0;
        _score.text = _scoreValue.ToString();
    }
}
