using System;
using System.Collections.Generic;
using UnityEngine;

public class InputValidator
{
    public void CheckingEnteredData(Vector2 startPoint, Vector2 endPoint, List<Edge> listEdges)
    {
        if (listEdges == null && listEdges.Count < 2)
            throw new Exception("Необходимо настроить pathFinder и testEdges в инспекторе.");

        if (startPoint == endPoint)
            throw new Exception("StartPoint и EndPoint равны.");

        for (int i = 0; i < listEdges.Count; i++)
        {
            ChecingRectangle(listEdges[i].First);
            ChecingRectangle(listEdges[i].Second);
            if ((listEdges.Count - 1) != i)
                CheckeQualityRectangles(listEdges[i + 1].First, listEdges[i].Second);

            CheckingEdge(listEdges[i]);
        }
    }

    private void ChecingRectangle(Rectangle rectangle)
    {
        if (rectangle.Min.magnitude > rectangle.Max.magnitude)
            throw new Exception("rectangle.Min больше чем rectangle.Max"); 
        if (rectangle.Min.magnitude == rectangle.Max.magnitude)
            throw new Exception("rectangle.Min равен rectangle.Max");
        
        if (rectangle.Max.magnitude < 2)
            throw new Exception("rectangle.Max.magnitude меньше 1");
    }

    private void CheckeQualityRectangles(Rectangle first, Rectangle second)
    {
        if (first.Min.magnitude != second.Min.magnitude || first.Max.magnitude != second.Max.magnitude)
            throw new Exception("first должны быть равным second");
    }

    private void CheckingEdge(Edge edge)
    {
        if (edge.Start.magnitude > edge.End.magnitude)
            throw new Exception("edge.Start больше чем edge.End");
        
        if (edge.Start.x == edge.End.x)
        {
            if (edge.Start.y == edge.End.y || (edge.End.y - edge.Start.y) < 0.5f)
                throw new Exception("edge.Start равен edge.End или промежуток между ними меньше 0.5");
        }
        else if (edge.Start.x == edge.End.x || (edge.End.x - edge.Start.x) < 0.5f)
            throw new Exception("edge.Start равен edge.End или промежуток между ними меньше 0.5");
    }
}
