using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singletone<T> : MonoBehaviour where T: Singletone<T>
{
    public static T Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<T>();

            if (_instance == null)
            {
                var holderObject = new GameObject($"Singleton_{typeof(T)}");
                _instance = holderObject.AddComponent<T>();
                DontDestroyOnLoad(holderObject);
            }

            return _instance;
        }
    }

    public static T TryInstance
    {
        get { return _instance != null ? _instance : null; }
    }

    private static T _instance = null;
    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
            throw new Exception("Singletone two init!!!");

        _instance = (T)this;

        DontDestroyOnLoad(this.gameObject);
    }

    protected virtual void OnDestroy()
    {
//        Debug.Log($"OnDestroySingleton");
    }
}
