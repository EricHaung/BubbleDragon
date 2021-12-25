using System;
using UnityEngine;

public class BubbleRoot : MonoBehaviour
{
    public Action<GameObject> OnRootHit;

    private void Start()
    {
        BubbleGameManager.Instance.AssignBubbleRoot(this);
    }

    public void TryHit(GameObject hitObj)
    {
        if (hitObj.transform.position.y + BubbleFactory.BUBBLE_DISTANCE / 2f >= this.gameObject.transform.position.y)
            OnRootHit?.Invoke(hitObj);
    }
}
