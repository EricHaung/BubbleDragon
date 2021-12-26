using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public RectTransform rectTransform;
    public Action<Bullet> OnShooting;
    public Action<Bullet> OnShootingEnd;
    public bool IsShooting
    {
        private set { }
        get { return isShooting; }
    }
    private int bulletType = -1;
    private bool isRainbow = false;
    private bool isShooting = false;
    private BounceMovement bounceMovement;
    private BulletPathPredictor bulletPathPredictor;

    private void Start()
    {
        bounceMovement = new BounceMovement(rectTransform, new Vector4(-226, 300, 226, -427));
        bulletPathPredictor = new BulletPathPredictor(new Vector4(-226, 300, 226, -427));
        BulletPool.Instance.AddBullet(this);
    }

    private void Update()
    {
        if (isShooting)
            OnShooting?.Invoke(this);
    }

    private void OnDestroy()
    {
        BulletPool.Instance.RemoveBullet(this);
    }

    public void Shoot(float speed, float dir)
    {
        isShooting = true;
        bounceMovement.SetDir(speed, dir);
    }

    public void SetBulletType(int _bulletType)
    {
        bulletType = _bulletType;

        if (bulletType == BubbleGameManager.Instance.bubbleSprites.Count)
            isRainbow = true;
        else
            isRainbow = false;

        if (isRainbow)
            this.GetComponent<UnityEngine.UI.Image>().sprite = BubbleGameManager.Instance.rainballSprite;//view
        else
            this.GetComponent<UnityEngine.UI.Image>().sprite = BubbleGameManager.Instance.bubbleSprites[bulletType];//view
    }

    public void Predict(float angle, float rootHeight)
    {
        bulletPathPredictor.Predict(this.transform.position, angle, rootHeight);
    }

    public void DisablePredict()
    {
        bulletPathPredictor.DisablePredict();
    }

    public int GetBulletType()
    {
        return bulletType;
    }

    public bool IsRainbow()
    {
        return isRainbow;
    }

    public Vector3 GetDir()
    {
        return bounceMovement.GetDir();
    }

    public void Reset()
    {
        isShooting = false;
        bulletType = -1;
        bounceMovement.Reset();
        OnShootingEnd?.Invoke(this);
    }
}
