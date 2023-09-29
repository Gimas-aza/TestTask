using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    public Vector2 startPoint;
    public Vector2 endPoint;
    public List<Edge> testEdges;
    private IEnumerable<Vector2> _path = new List<Vector2>();

    private void Start()
    {
        if (testEdges == null && testEdges.Count == 0)
        {
            Debug.LogError("Необходимо настроить pathFinder и testEdges в инспекторе.");
            return;
        }

        _path = GetPath(startPoint, endPoint, testEdges);
        _path = _path.Prepend(startPoint);
        _path = _path.Append(endPoint);

        if (_path != null)
        {
            Debug.Log("Найденный путь:");
            foreach (Vector2 point in _path)
            {
                // Debug.Log(point.x + " " + point.y);
            }
        }
        else
        {
            Debug.LogError("Путь не найден.");
        }
    }

    private void Update()
    {
        for (int i = 0; i < testEdges.Count(); i++)
        {
            DrawRectangle(testEdges[i].First);
            if (i == testEdges.Count() - 1)
                DrawRectangle(testEdges[i].Second);
            Debug.DrawLine(testEdges[i].Start, testEdges[i].End, Color.green);
        }
        
        for (int i = 0; i < _path.Count() - 1; i++)
        {
            Debug.DrawLine(_path.ToList()[i], _path.ToList()[i + 1], Color.red);
        }
    }

    private void DrawRectangle(Rectangle rectangle)
    {
        Debug.DrawLine(rectangle.Min, new Vector2(rectangle.Max.x, rectangle.Min.y), Color.blue);
        Debug.DrawLine(rectangle.Min, new Vector2(rectangle.Min.x, rectangle.Max.y), Color.blue);
        Debug.DrawLine(rectangle.Max, new Vector2(rectangle.Min.x, rectangle.Max.y), Color.blue);
        Debug.DrawLine(rectangle.Max, new Vector2(rectangle.Max.x, rectangle.Min.y), Color.blue);
    }

    private IEnumerable<Vector2> GetPath(Vector2 start, Vector2 end, IEnumerable<Edge> edges)
    {
        IEnumerable<Vector2> path = new List<Vector2>();
        Vector2 lastPoint = start;

        for (int i = 0; i < edges.Count() - 1; i++)
        {
            List<Vector2> points = GetPoints(i, lastPoint);
            path = path.Concat(points);
            lastPoint = points.Last();
        }

        Vector2 pointOnSegment = CalculatePointOnSegment(lastPoint, end, edges.Last());
        if (!CheckPointPassingOverEdge(pointOnSegment, edges.Last()))
        {
            lastPoint = GetLastPoint(edges.Last(), lastPoint);
            path = path.Append(lastPoint);
        }

        return path;
    }

    private List<Vector2> GetPoints(int indexEdge, Vector2 startPoint)
    {
        List<Vector2> listPoints = new();
        Vector2 middleNextEdgePoint;
        float middle = 0;
        Vector2 middleEdgePoint = GetMiddlePointEdge(testEdges[indexEdge], out float middleEdge);
        if (CheckLateralPositioningEdge(testEdges[indexEdge + 1]))
        {
            middleNextEdgePoint = GetMiddlePointEdge(testEdges[indexEdge + 1], out middle);
        }
        else
        {
            middle = GetMiddlePointRectangle(testEdges[indexEdge + 1].Second).x;
            Debug.Log(testEdges[indexEdge + 1].Start.x + "  " + middleEdge);
        }
        
        Vector2 point = CalculatePointOnRay(startPoint, middleEdgePoint, new Vector2(0, middle));
        if (point.magnitude > testEdges[indexEdge].Second.Max.magnitude || 
            point.magnitude < testEdges[indexEdge].Second.Min.magnitude)
        {
            point = GetMiddlePointRectangle(testEdges[indexEdge].First);
            listPoints.Add(point);
            listPoints.Add(CalculatePointOnRay(point, middleEdgePoint, new Vector2(0, middle)));
        }
        else
        {
            listPoints.Add(point);
        }

        return listPoints;
    }

    private Vector2 GetLastPoint(Edge edge, Vector2 startPoint)
    {
        Vector2 middleEdgePoint = GetMiddlePointEdge(edge, out float middleEdge);
        Vector2 middleRectanglePoint = GetMiddlePointRectangle(edge.Second);
        return CalculatePointOnRay(startPoint, middleEdgePoint, new Vector2(middleRectanglePoint.x, 0));
    }

    private Vector2 GetMiddlePointEdge(Edge edge, out float middleEdge) // TODO Переделать
    {
        middleEdge = edge.Start.y + ((edge.End.y - edge.Start.y) / 2);
        return new Vector2(edge.Start.x, middleEdge);
    }

    private Vector2 CalculatePointOnRay(Vector2 startPoint, Vector2 endPoint, Vector2 target)
    {
        if (startPoint.x == endPoint.x)
        {
            return new Vector2(target.x, startPoint.y);
        }

        float m = (float) (endPoint.y - startPoint.y) / (endPoint.x - startPoint.x);
        float x, y;
        if (target.x == 0)
        {
            x = startPoint.x + (target.y - startPoint.y) / m;
            return new Vector2(x, target.y);
        }
        else
        {
            y = startPoint.y + (target.x - startPoint.x) * m;
            return new Vector2(target.x, y);
        }

    }

    private Vector2 CalculatePointOnSegment(Vector2 startPoint, Vector2 endPoint, Edge edge)
    {
        float m = (float) (endPoint.y - startPoint.y) / (endPoint.x - startPoint.x);
        float x, y;
        if (CheckLateralPositioningEdge(edge))
        {
            y = startPoint.y + (edge.Start.x - startPoint.x) * m;
            return new Vector2(edge.Start.x, y);
        }
        else
        {
            x = startPoint.x + (edge.Start.y - startPoint.y) * m;
            return new Vector2(x, edge.Start.y);
        }

    }

    private Vector2 GetMiddlePointRectangle(Rectangle rectangle)
    {
        return new Vector2((rectangle.Min.x + rectangle.Max.x) / 2, (rectangle.Min.y + rectangle.Max.y) / 2);
    }

    private bool CheckLateralPositioningEdge(Edge edge)
    {
        if (edge.Start.x == edge.End.x)
            return true;
        else return false;
    }

    private bool CheckPointPassingOverEdge(Vector2 point, Edge edge)
    {
        if (CheckLateralPositioningEdge(edge) && point.y > edge.Start.y && point.y < edge.End.y)
            return true;
        else if (point.x > edge.Start.x && point.x < edge.End.x)
            return true;
        else 
            return false;
    }
}