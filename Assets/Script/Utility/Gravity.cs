using System;
using UnityEngine.UI;
using UnityEngine;

[RequireComponent(typeof(Image))]
public class Gravity : MonoBehaviour
{
    public Action OnBorderHit;
    private bool active;
    private Vector4 border; //left, up, right, down

    private RectTransform moveItem;
    private Vector3 acceleration;
    private Vector3 speed;

    public void SetUp(Vector4 _border, Vector3 _acceleration, Vector3 _speed)
    {
        if (moveItem == null)
            moveItem = this.gameObject.GetComponent<RectTransform>();

        active = false;
        border = _border;
        acceleration = _acceleration;
        speed = _speed;
    }

    public void SetGravityActive(bool _active)
    {
        active = _active;
        if (active)
            speed = acceleration * UnityEngine.Random.Range(0f, 0.5f);
        else
        {

        }
    }

    private void Update()
    {
        if (active)
        {
            speed += Time.deltaTime * acceleration;
            moveItem.localPosition += speed * Time.deltaTime;

            if (IsHitBorder() != 0)
            {
                OnBorderHit?.Invoke();
                SetGravityActive(false);
            }
        }
    }

    private int IsHitBorder()
    {
        if (moveItem.localPosition.x - moveItem.sizeDelta.x / 2f <= border.x)
            return 1;
        else if (moveItem.localPosition.y + moveItem.sizeDelta.y / 2f >= border.y)
            return 2;
        else if (moveItem.localPosition.x + moveItem.sizeDelta.x / 2f >= border.z)
            return 3;
        else if (moveItem.localPosition.y - moveItem.sizeDelta.y / 2f <= border.w)
            return 4;
        else
            return 0;
    }
}
