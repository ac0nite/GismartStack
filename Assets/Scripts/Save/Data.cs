using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable] 
public class Data
{
    public string DateTime { get; private set; }
    public List<Element> list = new List<Element>();
    public int Score { get ; set; }

    public void Add(Transform _transform, Color color)
    {
        list.Add(new Element(_transform, color));
    }

    public void UpdateDate()
    {
        DateTime = System.DateTime.Now.ToString();
    }

    public void Clear()
    {
        list.Clear();
        UpdateDate();
        Score = 0;
    }
}
