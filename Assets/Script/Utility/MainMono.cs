using System;
using UnityEngine;

public class MainMono : MonoBehaviour
{
    public static MainMono Instance;
    public Action OnUpdate;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Update()
    {
        OnUpdate?.Invoke();
    }
}
