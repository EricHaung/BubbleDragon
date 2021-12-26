using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BubbleFactory
{
    public static readonly float BUBBLE_DISTANCE = 44; //因為素材圖片可能不是滿版，所以生出來的圖片可能看起來會有間距，調整這個參數可以讓泡泡的碰撞判定距離變近，生成的泡泡也可以靠得比較近
    public static readonly float BUBBLE_SIZE = 46; //圖片生成尺寸
    public static readonly float ROCK_BUBBLE_SPAWN_RATE = 0.1f; //圖片生成尺寸

    private Material bubbleMaterial;
    private Transform bubbleParent;
    private RectTransform spawnRange;
    private List<Sprite> bubbleSprites;
    private Sprite rockSprite;

    private float difficulty = 1;
    private Vector2 initialPosition;
    private int line;

    public BubbleFactory(float _difficulty, List<Sprite> _bubbleSprites, Sprite _rockSprite, Transform _bubbleParent, RectTransform _spawnRange)
    {
        difficulty = _difficulty;
        bubbleSprites = _bubbleSprites;
        rockSprite = _rockSprite;
        bubbleParent = _bubbleParent;
        spawnRange = _spawnRange;
        CreateBubbleMaterial();
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
                if (Random.Range(0f, 1f) > ROCK_BUBBLE_SPAWN_RATE || line == 0)
                    CreateBubble(currentPos, false, (line == 0), (int)Mathf.Floor(Random.Range(0, bubbleSprites.Count)));
                else
                    CreateRock(currentPos, false, (line == 0));
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
        Image bubbleImg = bubbleObj.AddComponent<Image>();
        bubbleImg.sprite = bubbleSprites[type];
        bubbleImg.material = bubbleMaterial;
        RectTransform bubbleTransform = bubbleObj.GetComponent<RectTransform>();
        bubbleTransform.SetParent(bubbleParent);
        bubbleTransform.SetSiblingIndex(0);
        if (isWorldSpace)
            bubbleTransform.position = position;
        else
            bubbleTransform.localPosition = position;

        bubbleTransform.sizeDelta = Vector2.one * BUBBLE_SIZE;
        bubbleTransform.localScale = Vector3.one;

        Bubble bubble = bubbleObj.AddComponent<Bubble>();
        bubble.SetUp(isRoot, type);
        BubbleManager.Instance.AddBubble(bubble);

        return bubble;
    }

    public Bubble CreateRock(Vector2 position, bool isWorldSpace, bool isRoot)
    {
        GameObject bubbleObj = new GameObject("Bubble");
        bubbleObj.AddComponent<Image>().sprite = rockSprite;
        RectTransform bubbleTransform = bubbleObj.GetComponent<RectTransform>();
        bubbleTransform.SetParent(bubbleParent);
        bubbleTransform.SetSiblingIndex(0);
        if (isWorldSpace)
            bubbleTransform.position = position;
        else
            bubbleTransform.localPosition = position;

        bubbleTransform.sizeDelta = Vector2.one * BUBBLE_SIZE;
        bubbleTransform.localScale = Vector3.one;

        Bubble bubble = bubbleObj.AddComponent<Bubble>();
        bubble.SetUp(isRoot, -1);
        BubbleManager.Instance.AddBubble(bubble);

        return bubble;
    }

    public void UpdateBubbleSprite(Bubble bubble, int type)
    {
        bubble.gameObject.GetComponent<Image>().sprite = bubbleSprites[type];
    }

    public void SetGrayScale(bool active)
    {
        if (active)
            bubbleMaterial.SetFloat("_UseGrayScale", 1);
        else
            bubbleMaterial.SetFloat("_UseGrayScale", 0);
    }

    private void CreateBubbleMaterial()
    {
        bubbleMaterial = new Material(Shader.Find("BubbleGame/GrayScale"));
    }

}
