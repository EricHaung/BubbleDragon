using System.Collections;
using UnityEngine;

public class BubbleGameManager : MonoBehaviour
{
    public static BubbleGameManager Instance;
    private ShooterMediator shooterMediator;
    private Piner piner;
    private Bullet currentBullet;
    private bool allowShoot = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        piner = new Piner();
    }

    private void OnDestroy()
    {
        piner.Dispose();
        piner = null;
    }

    public void AssignShooterMediator(ShooterMediator _shooterMediator)
    {
        shooterMediator = _shooterMediator;
        shooterMediator.OnShootBubble += ShootBubble;
        shooterMediator.OnReloadFinished += ReloadBubbleFinished;
        shooterMediator.SetProperty(0.5f, 1);
    }

    private void ShootBubble(float dir)
    {
        if (currentBullet != null)
        {
            piner.SetPin(false);
            currentBullet.Shoot(10, dir);
            currentBullet.OnShootingEnd += BulletReset;
            currentBullet = null;
            allowShoot = false;
        }
    }

    public void RemoveShooterMediator()
    {
        shooterMediator = null;
    }

    public bool IsShootAllow(){
        return allowShoot;
    }

    private void ReloadBubbleFinished()
    {
        if (currentBullet == null && BulletPool.Instance.GetBulletCout() > 0)
        {
            StartCoroutine(TryGetBullet(0.5f));
        }
    }

    private void BulletReset(Bullet bullet)
    {
        bullet.transform.localPosition = new Vector3(312, -200, 0);
        bullet.OnShootingEnd -= BulletReset;
    }

    private IEnumerator TryGetBullet(float time)
    {
        Bullet idleBullet = BulletPool.Instance.GetCurrentIdleBullet();
        WaitForSeconds cooldownTime = new WaitForSeconds(time);
        while (idleBullet == null)
        {
            yield return cooldownTime;
            idleBullet = BulletPool.Instance.GetCurrentIdleBullet();
        }

        currentBullet = idleBullet;
        piner.SetTargetObject(idleBullet.gameObject, shooterMediator.GetPinSpot());
        piner.SetPin(true);
        allowShoot = true;
    }
}
