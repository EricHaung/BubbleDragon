﻿using UnityEngine;

public class BounceMovement
{
    private Vector4 border; //left, up, right, down
    private RectTransform moveItem;
    private Vector3 dir;

    private int hitBorderType = 0; // local parameter use in Update()
    private float overDis; // local parameter use in DistanceFix()

    public BounceMovement(RectTransform _moveItem, Vector4 _border)
    {
        border = _border;
        moveItem = _moveItem;
        dir = Vector3.zero;
        MainMono.Instance.OnUpdate += UpdatePosition;
    }

    public void SetDir(float speed, float angle)
    {
        dir.x = speed * Mathf.Cos(angle);
        dir.y = speed * Mathf.Sin(angle);
    }

    public Vector3 GetDir()
    {
        return dir;
    }

    public void Reset()
    {
        dir = Vector3.zero;
    }

    private void UpdatePosition()
    {
        if (dir == Vector3.zero)
            return;

        moveItem.localPosition += dir * Time.deltaTime;

        hitBorderType = IsHitXBorder();
        if (hitBorderType != 0)
        {
            DistanceFix(hitBorderType);
            dir.x *= -1;
        }

        hitBorderType = IsHitYBorder();
        if (hitBorderType != 0)
        {
            DistanceFix(hitBorderType);
            dir.y *= -1;
        }
    }

    private int IsHitXBorder()
    {
        if (moveItem.localPosition.x - moveItem.sizeDelta.x / 2f <= border.x)
            return 1;
        else if (moveItem.localPosition.x + moveItem.sizeDelta.x / 2f >= border.z)
            return 3;
        else
            return 0;
    }

    private int IsHitYBorder()
    {
        if (moveItem.localPosition.y + moveItem.sizeDelta.y / 2f >= border.y)
            return 2;
        else if (moveItem.localPosition.y - moveItem.sizeDelta.y / 2f <= border.w)
            return 4;
        else
            return 0;
    }

    private void DistanceFix(int mode)
    {
        if (dir == Vector3.zero)
            return;

        if (mode == 1)
        {
            overDis = border.x - (moveItem.localPosition.x - moveItem.sizeDelta.x / 2f);
            moveItem.localPosition -= new Vector3(-overDis, (overDis / dir.x) * dir.y, 0);
        }
        else if (mode == 2)
        {
            overDis = (moveItem.localPosition.y + moveItem.sizeDelta.y / 2f) - border.y;
            moveItem.localPosition -= new Vector3((overDis / dir.y) * dir.x, overDis, 0);
        }
        else if (mode == 3)
        {
            overDis = (moveItem.localPosition.x + moveItem.sizeDelta.x / 2f) - border.z;
            moveItem.localPosition -= new Vector3(overDis, (overDis / dir.x) * dir.y, 0);
        }
        else if (mode == 4)
        {
            overDis = border.w - (moveItem.localPosition.y - moveItem.sizeDelta.y / 2f);
            moveItem.localPosition -= new Vector3((overDis / dir.y) * dir.x, -overDis, 0);
        }
    }
}
