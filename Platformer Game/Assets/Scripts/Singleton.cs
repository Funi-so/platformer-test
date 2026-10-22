using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    public static T GetInstance()
    {
        if(_instance == null)
        {
            _instance = FindAnyObjectByType(typeof(T)) as T;
            if(_instance == null)
                _instance = new GameObject().AddComponent<T>();
        }
        return _instance;
    }

    void Awake()
    {
        if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
