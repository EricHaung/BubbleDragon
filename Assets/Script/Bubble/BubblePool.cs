using System;
using System.Collections.Generic;
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

    private void SetUp()
    {
        bubbles = new List<Bubble>();
    }

    public List<Bubble> GetBubbles()
    {
        return bubbles;
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

    public void RemoveBubble(Bubble bubble)
    {
        if (bubbles.Contains(bubble))
            bubbles.Remove(bubble);
    }

    public void DropDownBubbles()
    {
        
    }

}
