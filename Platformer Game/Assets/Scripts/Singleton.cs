using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T GetInstance()
    {
        if(_instance == null)
        {
            _instance = new GameObject().AddComponent<T>();
        }
        return _instance;
    }

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this as T;
        }
    }
}
