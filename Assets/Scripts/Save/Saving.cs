using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public void Append(Transform block, Color color)
    {
        Data.Add(block, color);
    }

    public void Append(Color from, Color to)
    {
        Data.fromColor = from;
        Data.toColor = to;
    }

    public void Clear()
    {
        Data.Clear();
    }

    public void Write()
    {
        Data.UpdateDate();
        string content = JsonUtility.ToJson(Data);
        string path = Application.persistentDataPath + "/" + _file;
        Debug.Log($"Path Save: {path}");
        File.Delete(path);
        File.WriteAllText(path, content);
    }

    public bool Read()
    {
        string path = Application.persistentDataPath + "/" + _file;
        if (File.Exists(path))
        {
            string content = File.ReadAllText(path);
            Data.Clear();
            Data = JsonUtility.FromJson<Data>(content);
            return true;
        }
        else
        {
            Debug.Log($"File not found {path}");
        }

        return false;
    }
}
