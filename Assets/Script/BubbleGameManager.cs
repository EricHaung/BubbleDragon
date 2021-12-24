using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGameManager : MonoBehaviour
{
    public static readonly int BUBBLE_FALL_DOWN_COUNT = 5;
    public static readonly int ELIMINATE_NUMBER = 3;
    public static BubbleGameManager Instance;

    [HeaderAttribute("Bubble Property")]
    public List<Sprite> bubbleSprites;
    public Transform bubbleRoot;
    public RectTransform spawnRange;
    private BubbleFactory bubbleFactory;
    private ShooterMediator shooterMediator;
    private Piner piner;
    private Bullet currentBullet;
    private bool allowShoot = false;
    private int shootTime = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
        piner = new Piner();
    }

    private void Start()
    {
        InitLevel(0.5f);
    }

    private void OnDestroy()
    {
        piner.Dispose();
        piner = null;
    }

    public void InitLevel(float difficulty)
    {
        bubbleFactory = new BubbleFactory(difficulty, bubbleSprites, bubbleRoot, spawnRange);
        bubbleFactory.CreateBubbles();
        BubbleManager.Instance.OnBubbleHit += OnBubbleHit;
    }

    public void OnBubbleHit(Bubble hitBubble, GameObject hitObj)
    {
        Bullet bullet = hitObj.GetComponent<Bullet>();
        Bubble newBubble = bubbleFactory.CreateBubble(PositionFix(hitBubble, bullet), true, false, bullet.GetBulletType());
        // 需要更好的位置計算法
        bullet.Reset();

        List<Bubble> sameColorBubbles = newBubble.GetAllSameTypeNeighbour(true, new List<Bubble>());
        if (sameColorBubbles.Count >= ELIMINATE_NUMBER)
            foreach (var bubble in sameColorBubbles)
            {
                bubble.OnEliminate();
            }
    }

    private Vector3 PositionFix(Bubble bubble, Bullet bullet)
    {
        float offset = BubbleFactory.BUBBLE_DISTANCE - Vector3.Distance(bubble.transform.position, bullet.transform.position);
        Vector3 originPos = bullet.transform.position;
        Vector3 direction = bullet.GetDir();
        float ratio = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);
        Vector3 finalPos = originPos - new Vector3((offset / ratio) * direction.x, (offset / ratio) * direction.y, 0);
        return finalPos;
    }

    public void AssignShooterMediator(ShooterMediator _shooterMediator)
    {
        shooterMediator = _shooterMediator;
        shooterMediator.OnShootBubble += ShootBubble;
        shooterMediator.OnReloadFinished += ReloadBubbleFinished;
        shooterMediator.SetProperty(0.5f, 0.5f);
    }

    public bool IsShootAllow()
    {
        return allowShoot;
    }

    private void ShootBubble(float dir)
    {
        if (currentBullet != null)
        {
            piner.SetPin(false);
            currentBullet.Shoot(5, dir);
            currentBullet.OnShooting += OnShooting;
            currentBullet.OnShootingEnd += BulletReset;
            currentBullet = null;
            allowShoot = false;
            shootTime += 1;
        }
    }

    public void OnShooting(Bullet bullet)
    {
        BubbleManager.Instance.TryHit(bullet.gameObject);
    }

    private void BulletReset(Bullet bullet)
    {
        bullet.transform.localPosition = new Vector3(312, -200, 0);
        bullet.OnShooting -= OnShooting;
        bullet.OnShootingEnd -= BulletReset;

        if (shootTime >= BUBBLE_FALL_DOWN_COUNT)
        {
            BubbleManager.Instance.DropDownBubbles();
            bubbleFactory.bubbleRoot.transform.localPosition += Vector3.down * BubbleFactory.BUBBLE_DISTANCE / 2f;
            shootTime = 0;
        }
    }

    public void RemoveShooterMediator()
    {
        shooterMediator = null;
    }

    private void ReloadBubbleFinished()
    {
        if (currentBullet == null && BulletPool.Instance.GetBulletCout() > 0)
        {
            StartCoroutine(TryGetBullet(0.5f));
        }
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
        currentBullet.SetBulletType(Random.Range(0, bubbleSprites.Count));
        piner.SetTargetObject(idleBullet.gameObject, shooterMediator.GetPinSpot());
        piner.SetPin(true);
        allowShoot = true;
    }
}
