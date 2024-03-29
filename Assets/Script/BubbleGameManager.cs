﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BubbleGameEvent;

public class BubbleGameManager : MonoBehaviour
{
    public static readonly float DIFFICULTY = 0.5f;
    public static readonly int BUBBLE_FALL_DOWN_COUNT = 5;
    public static readonly int ELIMINATE_NUMBER = 3;
    public static readonly float SHOOTER_ROTATE_SPEED = 50f;
    public static readonly float SHOOTER_RELOAD_SPEED = 0.5f;
    public static readonly float RAINBOW_BULLET_RATE = 0.6f;
    public static readonly float CRAB_SHOW_RATE = 0.03f;
    public static readonly float BULLET_SPEED = 1200f;
    private static BubbleGameManager instance;
    public static BubbleGameManager Instance
    {
        private set { }
        get { return instance; }
    }

    [HeaderAttribute("Level Property")]
    public Text finishedText;
    public Text failText;
    public RectTransform loseLine;
    public Animator crabAnimator;

    [HeaderAttribute("Bubble Property")]
    public List<Sprite> bubbleSprites;
    public Sprite rockSprite; //bullet never use this type of bubble
    public Sprite rainballSprite; //can use as any type of bubble
    public Transform bubbleParent;
    public RectTransform spawnRange;


    [HeaderAttribute("Preview Property")]
    public Image previewBulletImg;
    public Image holdBulletImg;
    private BubbleRoot bubbleRoot;
    private ShooterMediator shooterMediator;
    private Piner piner;
    private Bullet currentBullet;
    private bool allowShoot = false;
    private bool crabShowing = false;
    private int shootTime = 0;

    private int nextBulletType = -1;
    private int holdType = -1;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        piner = new Piner();
    }

    private void Start()
    {
        InitLevel(DIFFICULTY);
    }

    private void Update()
    {
        if (shooterMediator != null && currentBullet != null)
            currentBullet.Predict(shooterMediator.GetAngle(), bubbleRoot.gameObject.transform.position.y);
    }

    private void OnDestroy()
    {
        piner.Dispose();
        piner = null;
    }

    public void InitLevel(float difficulty)
    {
        allowShoot = false;
        crabShowing = false;
        shootTime = 0;
        finishedText.transform.localPosition = new Vector3(480, -27, 0);
        failText.transform.localPosition = new Vector3(480, -27, 0);
        holdType = -1;
        holdBulletImg.sprite = rockSprite;
        nextBulletType = (int)Mathf.Floor(Random.Range(0, bubbleSprites.Count));
        previewBulletImg.sprite = bubbleSprites[nextBulletType];
        BubbleManager.Instance.CreatNewGame(difficulty, bubbleSprites, rockSprite, bubbleParent, spawnRange);
        BubbleManager.Instance.OnBubbleHit += OnBubbleHit;
        EventListener.Instance.Observer += OnEventFire;
    }

    public void AssignBubbleRoot(BubbleRoot _bubbleRoot)
    {
        bubbleRoot = _bubbleRoot;
        bubbleRoot.OnRootHit += OnRootHit;
    }

    public void OnRootHit(GameObject hitObj)
    {
        Bullet bullet = hitObj.GetComponent<Bullet>();
        Bubble newBubble = BubbleManager.Instance.GetEmptyBubble(new Vector3(hitObj.transform.position.x, bubbleParent.position.y - BubbleFactory.BUBBLE_DISTANCE / 2f, 0), true, true, bullet.IsRainbow() ? (int)Mathf.Floor(Random.Range(0, bubbleSprites.Count)) : bullet.GetBulletType());
        // 需要更好的位置計算法(Ex:吸附、多泡泡考慮)
        bullet.Reset();

        List<Bubble> sameColorBubbles = newBubble.GetAllSameTypeNeighbour(true, new List<Bubble>());
        if (sameColorBubbles.Count >= ELIMINATE_NUMBER)
            foreach (var bubble in sameColorBubbles)
            {
                bubble.OnEliminate();
            }

        CheckResult();
    }

    public void OnBubbleHit(Bubble hitBubble, GameObject hitObj)
    {
        Bullet bullet = hitObj.GetComponent<Bullet>();
        int type = bullet.IsRainbow() ? hitBubble.GetBubbleType() : bullet.GetBulletType();
        type = type == -1 ? (int)Mathf.Floor(Random.Range(0, bubbleSprites.Count)) : type;

        Bubble newBubble = BubbleManager.Instance.GetEmptyBubble(PositionFix(hitBubble.transform.position, bullet), true, false, type);
        // 需要更好的位置計算法(Ex:吸附、多泡泡考慮)
        bullet.Reset();

        List<Bubble> sameColorBubbles = newBubble.GetAllSameTypeNeighbour(true, new List<Bubble>());
        if (sameColorBubbles.Count >= ELIMINATE_NUMBER)
            foreach (var bubble in sameColorBubbles)
            {
                bubble.OnEliminate();
            }
        CheckResult();
    }

    private Vector3 PositionFix(Vector3 hitPosition, Bullet bullet)
    {
        float offset = BubbleFactory.BUBBLE_DISTANCE - Vector3.Distance(hitPosition, bullet.transform.position);
        Vector3 originPos = bullet.transform.position;
        Vector3 direction = bullet.GetDir();
        float ratio = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);
        Vector3 finalPos = originPos - new Vector3((offset / ratio) * direction.x, (offset / ratio) * direction.y, 0);
        return finalPos;
    }

    private void CheckResult()
    {
        if (BubbleManager.Instance.GetRemainingBubbles().Count == 0)
        {
            finishedText.transform.localPosition = new Vector3(0, -27, 0);
            MainMono.Instance.Reset();
        }
        else if (BubbleManager.Instance.GetLowestPosY() < loseLine.transform.position.y)
        {
            failText.transform.localPosition = new Vector3(0, -27, 0);
            MainMono.Instance.Reset();
        }
    }

    public void AssignShooterMediator(ShooterMediator _shooterMediator)
    {
        shooterMediator = _shooterMediator;
        shooterMediator.OnShootBubble += ShootBubble;
        shooterMediator.OnReloadFinished += ReloadBubbleFinished;
        shooterMediator.OnSwitchBullet += OnSwitchBullet;
        shooterMediator.SetProperty(SHOOTER_RELOAD_SPEED, SHOOTER_ROTATE_SPEED);
    }

    public bool IsShootAllow()
    {
        return allowShoot;
    }

    private void ShootBubble(float dir)
    {
        if (allowShoot)
        {
            piner.SetPin(false);
            currentBullet.Shoot(BULLET_SPEED, dir);
            currentBullet.OnShooting += OnShooting;
            currentBullet.OnShootingEnd += BulletReset;
            currentBullet.DisablePredict();
            currentBullet = null;
            allowShoot = false;
            shootTime += 1;

            if (Random.Range(0f, 1f) > (1f - CRAB_SHOW_RATE) && !crabShowing)
            {
                crabAnimator.SetTrigger("ShowCrab");
            }
        }
    }

    public void OnShooting(Bullet bullet)
    {
        BubbleManager.Instance.TryHit(bullet.gameObject);
        bubbleRoot?.TryHit(bullet.gameObject);
    }

    private void BulletReset(Bullet bullet)
    {
        bullet.transform.localPosition = new Vector3(312, -200, 0);
        bullet.OnShooting -= OnShooting;
        bullet.OnShootingEnd -= BulletReset;

        if (shootTime >= BUBBLE_FALL_DOWN_COUNT)
        {
            if (crabShowing)
            {
                crabShowing = false;
                BubbleManager.Instance.SetBubbleGrayScale(false);
            }
            bubbleParent.transform.localPosition += Vector3.down * BubbleFactory.BUBBLE_DISTANCE / 2f;
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

        piner.SetTargetObject(idleBullet.gameObject, shooterMediator.GetPinSpot());
        piner.SetPin(true);
        yield return null;
        currentBullet = idleBullet;
        currentBullet.SetBulletType(nextBulletType);

        nextBulletType = (int)Mathf.Floor(Random.Range(0, bubbleSprites.Count));
        if (Random.Range(0f, RAINBOW_BULLET_RATE) > 0.5f)
        {
            nextBulletType = bubbleSprites.Count;
            previewBulletImg.sprite = rainballSprite;
        }
        else
        {
            previewBulletImg.sprite = bubbleSprites[nextBulletType];
        }
        allowShoot = true;
    }

    private void OnSwitchBullet()
    {
        if (!allowShoot)
            return;

        if (holdType != -1)
        {
            int tempType = holdType;
            holdType = currentBullet.GetBulletType();
            currentBullet.SetBulletType(tempType);
            holdBulletImg.sprite = holdType == bubbleSprites.Count ? rainballSprite : bubbleSprites[holdType];
        }
        else
        {
            holdType = currentBullet.GetBulletType();
            holdBulletImg.sprite = holdType == bubbleSprites.Count ? rainballSprite : bubbleSprites[holdType];
            currentBullet.SetBulletType(nextBulletType);

            nextBulletType = (int)Mathf.Floor(Random.Range(0, bubbleSprites.Count));
            if (Random.Range(0f, RAINBOW_BULLET_RATE) > 0.5f)
            {
                nextBulletType = bubbleSprites.Count;
                previewBulletImg.sprite = rainballSprite;
            }
            else
            {
                previewBulletImg.sprite = bubbleSprites[nextBulletType];
            }
        }
    }

    private void OnEventFire(BubbleGameEvent.EventType type)
    {
        if (type == BubbleGameEvent.EventType.HideColor && !crabShowing)
        {
            crabShowing = true;
            BubbleManager.Instance.SetBubbleGrayScale(true);
        }
    }

}
