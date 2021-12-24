using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Action<Bullet> OnShooting;
    public Action<Bullet> OnShootingEnd;
    public bool IsShooting
    {
        private set { }
        get { return isShooting; }
    }
    private int bulletType = -1;
    private bool isShooting = false;
    private BounceMovement bounceMovement;

    private void Start()
    {
        bounceMovement = this.gameObject.GetComponent<BounceMovement>();
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
        this.GetComponent<UnityEngine.UI.Image>().sprite = BubbleGameManager.Instance.bubbleSprites[bulletType];//view
    }

    public int GetBulletType()
    {
        return bulletType;
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
