using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InputValidator
{
    public bool CheckingEnteredData(Vector2 startPoint, Vector2 endPoint, List<Edge> listEdges)
    {
        if (listEdges == null && listEdges.Count < 2)
        {
            Debug.LogError("Необходимо настроить pathFinder и testEdges в инспекторе.");
            return false;
        }

        if (startPoint == endPoint)
        {
            Debug.LogError("StartPoint и EndPoint равны.");
            return false;
        }

        if (startPoint.x < listEdges[0].First.Min.x || startPoint.x > listEdges[0].First.Max.x 
            || startPoint.y < listEdges[0].First.Min.y || startPoint.y > listEdges[0].First.Max.y)
        {
            Debug.LogError("StartPoin выходит за рамки квадрата");
            return false;
        }

        if (endPoint.x < listEdges.Last().Second.Min.x || endPoint.x > listEdges.Last().Second.Max.x 
            || endPoint.y < listEdges.Last().Second.Min.y || endPoint.y > listEdges.Last().Second.Max.y)
        {
            Debug.Log(startPoint.x + " " + listEdges.Last().Second.Min.x);
            Debug.LogError("EndPoint выходит за рамки квадрата");
            return false;
        }

        for (int i = 0; i < listEdges.Count; i++)
        {
            if (!ChecingRectangle(listEdges[i].First)) return false;
            if (!ChecingRectangle(listEdges[i].Second)) return false;

            if ((listEdges.Count - 1) != i)
                if (!CheckeQualityRectangles(listEdges[i + 1].First, listEdges[i].Second)) return false;

            if (!CheckingEdge(listEdges[i])) return false;
        }

        return true;
    }

    private bool ChecingRectangle(Rectangle rectangle)
    {
        if (rectangle.Min.x > rectangle.Max.x || rectangle.Min.y > rectangle.Max.y)
        {
            Debug.LogError("rectangle.Min больше чем rectangle.Max"); 
            return false;
        }
        if (rectangle.Min.x == rectangle.Max.x || rectangle.Min.y == rectangle.Max.y)
        {
            Debug.LogError("rectangle.Min равен rectangle.Max");
            return false;
        }
        if ((rectangle.Max.x - rectangle.Min.x) < 1 || (rectangle.Max.y - rectangle.Min.y) < 1)
        {
            Debug.LogError("rectangle меньше 1");
            return false;
        }
        return true;
    }

    private bool CheckeQualityRectangles(Rectangle first, Rectangle second)
    {
        if (first.Min.magnitude != second.Min.magnitude || first.Max.magnitude != second.Max.magnitude)
        {
            Debug.LogError("first должны быть равным second");
            return false;
        }
        return true;
    }

    private bool CheckingEdge(Edge edge)
    {
        if (edge.Start.magnitude > edge.End.magnitude)
        {
            Debug.LogError("edge.Start больше чем edge.End");
            return false;
        }
        if (edge.Start.x == edge.End.x)
        {
            if (edge.Start.y == edge.End.y || (edge.End.y - edge.Start.y) < 0.5f)
            {
                Debug.LogError("edge.Start равен edge.End или промежуток между ними меньше 0.5");
                return false;
            }
        }
        else if (edge.Start.x == edge.End.x || (edge.End.x - edge.Start.x) < 0.5f)
        {
            Debug.LogError("edge.Start равен edge.End или промежуток между ними меньше 0.5");
            return false;
        }
        return true;
    }
}
