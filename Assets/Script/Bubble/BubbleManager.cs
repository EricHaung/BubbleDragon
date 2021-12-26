using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BubbleManager
{
    public Action<Bubble, GameObject> OnBubbleHit;
    public static BubbleManager Instance
    {
        private set { }
        get
        {
            if (instance == null)
            {
                instance = new BubbleManager();
                instance.SetUp();
            }
            return instance;
        }
    }
    private static BubbleManager instance;
    private List<Bubble> bubbles;
    private Queue<Bubble> idleBubbles;
    private BubbleFactory bubbleFactory;

    private void SetUp()
    {
        bubbles = new List<Bubble>();
        idleBubbles = new Queue<Bubble>();
    }

    public void CreatNewGame(float difficulty, List<Sprite> bubbleSprites, Sprite rockSprite, Transform bubbleParent, RectTransform spawnRange)
    {
        bubbleFactory = new BubbleFactory(difficulty, bubbleSprites, rockSprite, bubbleParent, spawnRange);
        bubbleFactory.CreateBubbles();
    }

    public Bubble GetEmptyBubble(Vector2 position, bool isWorldSpace, bool isRoot, int type)
    {
        if (idleBubbles.Count > 0)
        {
            Bubble idleBubble = idleBubbles.Dequeue();
            idleBubble.UpdatePostion(position, isWorldSpace);
            idleBubble.SetUp(isRoot, type);
            bubbleFactory.UpdateBubbleSprite(idleBubble, type);
            return idleBubble;
        }
        else
            return bubbleFactory.CreateBubble(position, isWorldSpace, isRoot, type);
    }

    public List<Bubble> GetBubbles()
    {
        return bubbles;
    }

    public List<Bubble> GetRemainingBubbles()
    {
        return bubbles.Where(item => !item.GetBubbleIdle()).ToList();
    }

    public float GetLowestPosY()
    {
        return bubbles.Where(item => !item.GetBubbleIdle()).Aggregate((result, next) => result.transform.position.y <= next.transform.position.y ? result : next).transform.position.y;
    }

    public void AddBubble(Bubble bubble)
    {
        bubbles.Add(bubble);
    }

    public void TryHit(GameObject hitObj)
    {
        for (int index = 0; index < bubbles.Count; index++)
            bubbles[index].TryHit(hitObj);
    }

    public void BubbleHit(Bubble hitBubble, GameObject hitObj)
    {
        OnBubbleHit?.Invoke(hitBubble, hitObj);
    }

    public void SetBubbleIdle(Bubble bubble)
    {
        if (bubbles.Contains(bubble))
        {
            bubble.SetBubbleIdle(true);

            if (!idleBubbles.Contains(bubble))
                idleBubbles.Enqueue(bubble);
        }
    }

    public void SetBubbleGrayScale(bool active)
    {
        bubbleFactory.SetGrayScale(active);
    }

    public void RemoveBubble(Bubble bubble)
    {
        if (bubbles.Contains(bubble))
            bubbles.Remove(bubble);
    }
}
