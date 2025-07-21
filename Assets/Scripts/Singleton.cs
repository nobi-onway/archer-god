using System;
using Netick.Unity;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance => SingletonExtension.GetSingleton(ref _instance);
}

public static class SingletonExtension
{
    public static T GetSingleton<T>(ref T instance) where T : Component
    {
        if (instance == null)
        {
            instance = GameObject.FindAnyObjectByType<T>();
            if (instance == null)
            {
                instance = new GameObject(typeof(T).Name).AddComponent<T>();
            }
        }
        return instance;
    }
}