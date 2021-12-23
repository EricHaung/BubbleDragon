using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Action<Bullet> OnShootingEnd;
    public bool IsShooting
    {
        private set { }
        get { return isShooting; }
    }
    private bool isShooting = false;
    private float resetTimer = 0;
    private BounceMovement bounceMovement;

    private void Start()
    {
        bounceMovement = this.gameObject.GetComponent<BounceMovement>();
        BulletPool.Instance.AddBullet(this);
    }

    private void Update()
    {
        if (isShooting)
        {
            resetTimer += Time.deltaTime;
            if (resetTimer >= 1)
            {
                resetTimer = 0;
                isShooting = false;
                OnShootingEnd?.Invoke(this);
                Reset();
            }
        }
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

    public void Reset()
    {
        bounceMovement.Reset();
    }
}
