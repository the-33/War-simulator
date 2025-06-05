using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NoDestroyList : MonoBehaviour
{
    public List<GameObject> list;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void destroyObjects(Action action)
    {
        foreach (GameObject obj in list) { Destroy(obj); }
        action();
        Destroy(gameObject);
    }
}
