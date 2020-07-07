using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Build.Content;
using UnityEngine;

public class Saving : Singletone<Saving>
{
    public Data Data { get; private set; }
    [SerializeField] private string _file = "MyListTest.json";
    protected override void Awake()
    {
        base.Awake();
        Data = new Data();
        Data.Clear();
    }

    public void Append(Transform block)
    {
        Data.Add(block);
    }

    public void Append(int score)
    {
        Data.Score = score;
    }

    public void Write()
    {
        Data.UpdateDate();
        string content = JsonUtility.ToJson(Data);
        string path = Application.persistentDataPath + "/" + _file;
        Debug.Log($"Path Save: {path}");
        File.WriteAllText(path, content);
    }

    public void Read()
    {
        string path = Application.persistentDataPath + "/" + _file;
        string content = File.ReadAllText(path);
        Data.Clear();
        Data = JsonUtility.FromJson<Data>(content);
    }
}
