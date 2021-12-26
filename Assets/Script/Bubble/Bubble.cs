using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private bool isRoot = false; //黏在底座的球
    private bool isIdle = false; //空閒中沒有被使用到
    private int type = -1;
    private bool isRainball;
    private List<Bubble> neighbourBubbles;
    private Gravity gravity;

    public void SetUp(bool _isRoot, int _type)
    {
        neighbourBubbles = new List<Bubble>();
        gravity = this.gameObject.AddComponent<Gravity>();
        gravity.SetUp(new Vector4(0, 0, 456, -730), new Vector3(0, -9.8f * 100f, 0), Vector3.zero);
        isIdle = false;
        isRoot = _isRoot;
        type = _type;
        CollectNeighbour();
    }

    public int GetBubbleType()
    {
        return type;
    }

    public void SetBubbleIdle(bool _isIdle)
    {
        isIdle = _isIdle;
    }

    public bool GetBubbleIdle()
    {
        return isIdle;
    }

    public List<Bubble> GetAllSameTypeNeighbour(bool isFirstBubble, List<Bubble> bubbleInclude)
    {
        List<Bubble> bubbles = new List<Bubble>();
        bubbles.Add(this);

        if (isFirstBubble)
            bubbleInclude.Add(this);

        List<Bubble> sameColorBubbles = neighbourBubbles.Where(item => item.type == this.type && !bubbleInclude.Contains(item)).ToList();

        bubbleInclude.AddRange(sameColorBubbles);

        foreach (var bubble in sameColorBubbles)
            bubbles.AddRange(bubble.GetAllSameTypeNeighbour(false, bubbleInclude)); //錯

        return bubbles;
    }

    public void TryHit(GameObject hitObj)
    {
        if (Vector3.Distance(this.transform.position, hitObj.transform.position) < BubbleFactory.BUBBLE_DISTANCE && !isIdle)
            BubbleManager.Instance.BubbleHit(this, hitObj);
    }

    public void OnEliminate()
    {
        UpdatePostion(new Vector3(630, -600, 0));
        BubbleManager.Instance.SetBubbleIdle(this);
        RemoveNeighbour();
    }

    private void CollectNeighbour()
    {
        foreach (var bubble in BubbleManager.Instance.GetBubbles())
        {
            if (bubble == this)
                continue;

            if (Vector3.Distance(this.transform.localPosition, bubble.transform.localPosition) <= BubbleFactory.BUBBLE_DISTANCE * 1.01f)
            {
                neighbourBubbles.Add(bubble);
                bubble.NotifyNeighbourCollect(this);
            }
        }
    }

    private void NotifyNeighbourCollect(Bubble bubble)
    {
        if (!neighbourBubbles.Contains(bubble))
            neighbourBubbles.Add(bubble);
    }

    public void UpdatePostion(Vector3 position, bool isWorldSpace = false)
    {
        if (isWorldSpace)
            this.gameObject.transform.position = position;
        else
            this.gameObject.transform.localPosition = position;
    }

    private void RemoveNeighbour()
    {
        foreach (var bubble in neighbourBubbles)
            bubble.NotifyNeighbourRemove(this);

        CheckHasRoot();

        neighbourBubbles = new List<Bubble>();
    }

    private void NotifyNeighbourRemove(Bubble bubble)
    {
        if (neighbourBubbles.Contains(bubble))
        {
            neighbourBubbles.Remove(bubble);
        }
    }

    private void CheckHasRoot()
    {
        if (neighbourBubbles.Count > 0)
            foreach (var testBubble in neighbourBubbles)
            {
                List<Bubble> leftBubble = new List<Bubble>();
                if (!testBubble.IsRootExist(leftBubble))
                {
                    leftBubble.ForEach(item => item.FallDown());
                }
            }
    }

    private bool IsRootExist(List<Bubble> bubbleInclude)
    {
        if (isRoot)
            return true;
        else
        {
            bubbleInclude.Add(this);
            List<Bubble> bubblesNeedTest = neighbourBubbles.Where(item => !bubbleInclude.Contains(item)).ToList();

            if (bubblesNeedTest.Count == 0)
                return false;

            bool result = false;
            foreach (var bubble in bubblesNeedTest)
                result = result || bubble.IsRootExist(bubbleInclude);

            return result;
        }
    }

    private void FallDown()
    {
        gravity.SetGravityActive(true);
        gravity.OnBorderHit += OnFallOutside;
        BubbleManager.Instance.SetBubbleIdle(this);
        RemoveNeighbour();
    }

    private void OnFallOutside()
    {
        gravity.OnBorderHit -= OnFallOutside;
        UpdatePostion(new Vector3(630, 200, 0));
    }

}
