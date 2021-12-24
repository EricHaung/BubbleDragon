using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleFactory
{
    public List<Sprite> bubbleSprites;
    public Transform bubbleRoot;
    public RectTransform spawnRange;

    public static readonly float BUBBLE_DISTANCE = 48; //因為素材圖片可能不是滿版，所以生出來的圖片可能看起來會有間距，調整這個參數可以讓泡泡的碰撞判定距離變近，生成的泡泡也可以靠得比較近
    public static readonly float BUBBLE_SIZE = 50; //圖片生成尺寸

    private float difficulty = 1;
    private Vector2 initialPosition;
    private int line;

    public BubbleFactory(float _difficulty, List<Sprite> _bubbleSprites, Transform _bubbleRoot, RectTransform _spawnRange)
    {
        difficulty = _difficulty;
        bubbleSprites = _bubbleSprites;
        bubbleRoot = _bubbleRoot;
        spawnRange = _spawnRange;
    }
    public void CreateBubbles()
    {
        initialPosition = new Vector2(BUBBLE_DISTANCE / 2f, -BUBBLE_DISTANCE / 2f);
        Vector2 currentPos = initialPosition;
        line = 0;
        while (BUBBLE_DISTANCE / 2f - currentPos.y < spawnRange.sizeDelta.y * difficulty)
        {
            while (currentPos.x + BUBBLE_DISTANCE / 2f < spawnRange.sizeDelta.x)
            {
                CreateBubble(currentPos, false, (line == 0), Random.Range(0, bubbleSprites.Count));
                currentPos.x += BUBBLE_DISTANCE;
            }
            currentPos.y -= BUBBLE_DISTANCE * Mathf.Sin(Mathf.Deg2Rad * 60);
            line++;
            currentPos.x = (line % 2 == 0) ? BUBBLE_DISTANCE / 2f : BUBBLE_DISTANCE;
        }
    }

    public Bubble CreateBubble(Vector2 position, bool isWorldSpace, bool isRoot, int type)
    {
        GameObject bubbleObj = new GameObject("Bubble");
        bubbleObj.AddComponent<Image>().sprite = bubbleSprites[type];
        RectTransform bubbleTransform = bubbleObj.GetComponent<RectTransform>();
        bubbleTransform.SetParent(bubbleRoot);
        bubbleTransform.SetSiblingIndex(0);
        if (isWorldSpace)
            bubbleTransform.position = position;
        else
            bubbleTransform.localPosition = position;

        bubbleTransform.sizeDelta = Vector2.one * BUBBLE_SIZE;
        bubbleTransform.localScale = Vector3.one;

        Bubble bubble = bubbleObj.AddComponent<Bubble>();
        bubble.SetUp(isRoot, type, bubbleTransform.localPosition);
        BubbleManager.Instance.AddBubble(bubble);

        return bubble;
    }
}
