using UnityEngine;

public class Piner
{
    public GameObject targetObj;
    public GameObject pinSpot;
    private bool isPin = false;
    
    public Piner()
    {
        MainMono.Instance.OnUpdate += UpdatePin;
    }

    public void Dispose()
    {
        MainMono.Instance.OnUpdate -= UpdatePin;
    }

    public void SetTargetObject(GameObject _targetObj, GameObject _pinSpot)
    {
        targetObj = _targetObj;
        pinSpot = _pinSpot;

    }

    public void SetPin(bool active)
    {
        isPin = active;
    }

    private void UpdatePin()
    {
        if (isPin)
        {
            targetObj.transform.position = pinSpot.transform.position;
        }
    }
}
