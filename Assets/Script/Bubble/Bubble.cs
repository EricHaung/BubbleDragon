using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private bool isRoot = false; //黏在底座的球
    private int type = -1;
    private Vector3 position;
    public List<Bubble> neighbourBubbles;

    private Gravity gravity;

    public void SetUp(bool _isRoot, int _type, Vector3 _position)
    {
        neighbourBubbles = new List<Bubble>();
        gravity = this.gameObject.AddComponent<Gravity>();
        gravity.SetUp(new Vector4(0, 0, 456, -730), new Vector3(0, -9.8f * 100f, 0), Vector3.zero);
        isRoot = _isRoot;
        type = _type;
        position = _position;
        CollectNeighbour();
    }

    public Vector3 GetPostion()
    {
        return position;
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
        if (Vector3.Distance(this.transform.position, hitObj.transform.position) < BubbleFactory.BUBBLE_DISTANCE)
            BubbleManager.Instance.BubbleHit(this, hitObj);
    }

    public void OnEliminate()
    {
        UpdatePostion(new Vector3(330, 250, 0));
        RemoveNeighbour();
    }

    private void OnFallOutside()
    {
        gravity.OnBorderHit -= OnFallOutside;
        UpdatePostion(new Vector3(330, 250, 0));
    }

    private void OnFallDown()
    {
        gravity.SetGravityActive(true);
        gravity.OnBorderHit += OnFallOutside;
        RemoveNeighbour();
    }

    private void CollectNeighbour()
    {
        foreach (var bubble in BubbleManager.Instance.GetBubbles())
        {
            if (bubble == this)
                continue;

            if (Vector3.Distance(position, bubble.GetPostion()) <= BubbleFactory.BUBBLE_DISTANCE * 1.01f)
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

    private void UpdatePostion(Vector3 _position)
    {
        position = _position;
        this.gameObject.transform.localPosition = position;
    }

    private void RemoveNeighbour()
    {
        foreach (var bubble in neighbourBubbles)
            bubble.NotifyNeighbourRemove(this);

        CheckHasRoot();

        neighbourBubbles = new List<Bubble>();
    }

    private void CheckHasRoot()
    {
        if (neighbourBubbles.Count > 0)
            foreach (var testBubble in neighbourBubbles)
            {
                List<Bubble> leftBubble = new List<Bubble>();
                if (!testBubble.IsRootExist(leftBubble))
                {
                    leftBubble.ForEach(item => item.OnFallDown());
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

    private void NotifyNeighbourRemove(Bubble bubble)
    {
        if (neighbourBubbles.Contains(bubble))
        {
            neighbourBubbles.Remove(bubble);
        }
    }

}
