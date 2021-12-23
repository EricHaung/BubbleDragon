using System;
using UnityEngine;

public class ShooterMediator
{
    public Action<float> OnShootBubble;
    public Action OnReloadFinished;

    private ShooterView view;
    private float reloadTime;
    private float rotataSpeed;
    private float loadingPregress;

    public void SetUp(ShooterView _view)
    {
        view = _view;
        reloadTime = 0;
        rotataSpeed = 0;
        loadingPregress = 0;
        MainMono.Instance.OnUpdate += UpdateProperty;
        BubbleGameManager.Instance.AssignShooterMediator(this);
    }

    public void Dispose()
    {
        MainMono.Instance.OnUpdate -= UpdateProperty;
        BubbleGameManager.Instance.RemoveShooterMediator();
    }

    public void SetProperty(float _reloadTime, float _rotataSpeed)
    {
        reloadTime = _reloadTime;
        rotataSpeed = _rotataSpeed;
    }

    public GameObject GetPinSpot()
    {
        return view.pinSpot;
    }

    private void UpdateProperty()
    {
        if (loadingPregress < 1)
        {
            loadingPregress += Time.deltaTime / reloadTime;
            view.UpdateReloadImg(loadingPregress);

            if (loadingPregress >= 1)
            {
                OnReloadFinished?.Invoke();
            }
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if ((view.GetAimerRotation() + 360) % 360 < 160)
                view.RotateAimer(rotataSpeed);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            if ((view.GetAimerRotation() + 360) % 360 > 20)
                view.RotateAimer(-rotataSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space) && loadingPregress >= 1 && BubbleGameManager.Instance.IsShootAllow())
        {
            loadingPregress = 0;
            OnShootBubble?.Invoke(Mathf.Deg2Rad * view.GetAimerRotation());
            view.OnShoot();
        }
    }

}
