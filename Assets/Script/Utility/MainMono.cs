using System;
using UnityEngine;

public class MainMono : MonoBehaviour
{
    public static MainMono Instance;
    public Action OnUpdate;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Update()
    {
        OnUpdate?.Invoke();
    }
}
