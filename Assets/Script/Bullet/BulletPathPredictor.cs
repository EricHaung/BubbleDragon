using BubbleGL;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BulletPathPredictor
{
    private Vector4 border; //left, up, right, down
    private Vector3 firstPosition;
    private Vector3 direct;
    private List<Vector3> pathNodes;
    private List<DottedLineGL> pathGLs;

    private bool hitRoot;// local parameter use in Predict()
    private int pathIdx;// local parameter use in Predict()
    private Vector3 midPoint; // local parameter use in GetHitBorder()
    float hitX; // local parameter use in GetHitBorder()
    float hitY; // local parameter use in GetHitBorder()

    public BulletPathPredictor(Vector4 _border)
    {
        border = _border;
        pathNodes = new List<Vector3>();
        pathGLs = new List<DottedLineGL>();
        direct = Vector3.zero;
    }

    public void Predict(Vector3 _firstPosition, float angle, float rootHeight)
    {
        pathNodes = new List<Vector3>();
        firstPosition = _firstPosition;
        direct.x = Mathf.Cos(angle);
        direct.y = Mathf.Sin(angle);
        pathIdx = 0;
        hitRoot = false;

        AddMidpoint(((1 / (direct.magnitude * direct.magnitude)) + (BubbleFactory.BUBBLE_DISTANCE / 2f)) * direct + firstPosition);
        while (GetHitBubble(pathIdx) == Vector3.zero)
        {
            if (GetHitRoot(pathIdx, rootHeight) != Vector3.zero)
            {
                AddMidpoint(GetHitRoot(pathIdx, rootHeight));
                hitRoot = true;
                break;
            }
            else
                AddMidpoint(GetHitBorder(pathIdx));

            pathIdx++;
            if (pathIdx > 8)
                break;
        }

        if (!hitRoot && pathIdx < 8)
            AddMidpoint(GetHitBubble(pathIdx));
    }

    public void DisablePredict()
    {
        Reset();
    }

    private Vector3 GetHitBubble(int index)
    {
        Bubble firstHitBbble = null;
        Vector3 hitPosition = Vector3.zero;
        Vector3 minHitPosition = Vector3.zero;
        float minDistance = -1;

        foreach (var bubble in BubbleManager.Instance.GetBubbles().Where(item => !item.GetBubbleIdle()))
        {
            Vector3 bubblePos = bubble.transform.position;
            float tempNumerator = Mathf.Abs(direct.y * bubblePos.x - direct.x * bubblePos.y - direct.y * pathNodes[index].x + direct.x * pathNodes[index].y);
            float tempDenominator = Mathf.Sqrt(direct.x * direct.x + direct.y * direct.y);
            float tempDistance = tempNumerator / tempDenominator;
            if (tempDistance <= BubbleFactory.BUBBLE_DISTANCE)
            {
                hitPosition = GetHitPosition(index, bubblePos, tempDistance);
                if (minDistance == -1)
                {
                    firstHitBbble = bubble;
                    minDistance = Vector3.Distance(hitPosition, pathNodes[index]);
                    minHitPosition = hitPosition;
                }
                else if (Vector3.Distance(hitPosition, pathNodes[index]) < minDistance)
                {
                    firstHitBbble = bubble;
                    minDistance = Vector3.Distance(hitPosition, pathNodes[index]);
                    minHitPosition = hitPosition;
                }
            }
        }

        if (firstHitBbble != null)
            return minHitPosition;

        else
            return Vector3.zero;
    }

    private Vector3 GetHitRoot(int index, float rootHeight)
    {
        midPoint = Vector3.zero;

        if (System.Math.Round(direct.y, 4) == 0f)
            return Vector3.zero;
        else if (direct.y <= 0)
            return Vector3.zero;
        else
        {
            hitY = rootHeight - (BubbleFactory.BUBBLE_DISTANCE / 2f);

            float X = (hitY - pathNodes[index].y) * (direct.x / direct.y) + pathNodes[index].x;

            if (X > border.z - (BubbleFactory.BUBBLE_DISTANCE / 2f) || X < border.x + (BubbleFactory.BUBBLE_DISTANCE / 2f))
                return Vector3.zero;
            else
                midPoint = new Vector3(X, hitY, 0);

            return midPoint;
        }
    }

    private Vector3 GetHitPosition(int index, Vector3 bubblePos, float minDistance)
    {
        float cutLength = Mathf.Sqrt(BubbleFactory.BUBBLE_DISTANCE * BubbleFactory.BUBBLE_DISTANCE - minDistance * minDistance) - (BubbleFactory.BUBBLE_DISTANCE / 2f);
        Vector3 projectPoint = bubblePos - pathNodes[index];

        return ((Vector3.Dot(projectPoint, direct) / (direct.magnitude * direct.magnitude)) - cutLength) * direct + pathNodes[index];
    }

    private Vector3 GetHitBorder(int index)
    {
        midPoint = Vector3.zero;

        if (System.Math.Round(direct.y, 4) == 0f)
        {
            midPoint = new Vector3(direct.x >= 0 ? border.z : border.x, pathNodes[index].y, 0);
            Redirection(new Vector3(direct.x * -1, direct.y, 0));
            return midPoint;
        }

        else if (System.Math.Round(direct.x, 4) == 0f)
        {
            midPoint = new Vector3(pathNodes[index].x, direct.y >= 0 ? border.y : border.w, 0);
            Redirection(new Vector3(direct.x, direct.y * -1, 0));
            return midPoint;
        }
        else
        {
            hitX = direct.x >= 0 ? border.z - (BubbleFactory.BUBBLE_DISTANCE / 2f) : border.x + (BubbleFactory.BUBBLE_DISTANCE / 2f);
            hitY = direct.y >= 0 ? border.y - (BubbleFactory.BUBBLE_DISTANCE / 2f) : border.w + (BubbleFactory.BUBBLE_DISTANCE / 2f);

            float Y = (hitX - pathNodes[index].x) * (direct.y / direct.x) + pathNodes[index].y;
            float X = (hitY - pathNodes[index].y) * (direct.x / direct.y) + pathNodes[index].x;

            if (Y > border.y - (BubbleFactory.BUBBLE_DISTANCE / 2f) || Y < border.w + (BubbleFactory.BUBBLE_DISTANCE / 2f))
            {
                Y = (X - pathNodes[index].x) * (direct.y / direct.x) + pathNodes[index].y;
                midPoint = new Vector3(X, Y, 0);
                Redirection(new Vector3(direct.x, direct.y * -1, 0));
            }
            else
            {
                X = (Y - pathNodes[index].y) * (direct.x / direct.y) + pathNodes[index].x;
                midPoint = new Vector3(X, Y, 0);
                Redirection(new Vector3(direct.x * -1, direct.y, 0));
            }
            return midPoint;
        }
    }

    private void Redirection(Vector3 newDirection)
    {
        direct = newDirection;
    }

    private void AddMidpoint(Vector3 point)
    {
        pathNodes.Add(point);
        UpdateLine();
    }

    private void UpdateLine()
    {
        int index = 0;
        for (index = 0; index < pathNodes.Count; index++)
        {
            if (index + 1 < pathNodes.Count)
            {
                if (index < pathGLs.Count)
                    pathGLs[index].UpdatePosition(NormalizedToScreen(pathNodes[index]), NormalizedToScreen(pathNodes[index + 1]));
                else
                    pathGLs.Add(new DottedLineGL(Color.white, NormalizedToScreen(pathNodes[index]), NormalizedToScreen(pathNodes[index + 1])));
            }
        }

        if (index < pathGLs.Count)
            for (int frontBack = pathGLs.Count - 1; frontBack >= index; frontBack--)
            {
                pathGLs[frontBack].Dispose();
                pathGLs.RemoveAt(frontBack);
            }

    }

    private Vector3 NormalizedToScreen(Vector3 position)
    {
        return new Vector3((position.x + Screen.width / 2f) / (float)Screen.width, (position.y + Screen.height / 2f) / (float)Screen.height, 0);
    }

    private void Reset()
    {
        foreach (var gl in pathGLs)
            gl.Dispose();

        pathNodes = new List<Vector3>();
        pathGLs = new List<DottedLineGL>();
        direct = Vector3.zero;
    }
}
