using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour, IPathFinder
{
    [SerializeField] private Vector2 _startPoint;
    [SerializeField] private Vector2 _endPoint;
    [SerializeField] private List<Edge> _listEdges;
    private IEnumerable<Vector2> _path = new List<Vector2>();

    private void Start()
    {
        CheckingEnteredData();

        _path = GetPath(_startPoint, _endPoint, _listEdges);
    }

    private void Update()
    {
        for (int i = 0; i < _listEdges.Count(); i++)
        {
            DrawRectangle(_listEdges[i].First);
            if (i == _listEdges.Count() - 1)
                DrawRectangle(_listEdges[i].Second);
            Debug.DrawLine(_listEdges[i].Start, _listEdges[i].End, Color.green);
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

    private void CheckingEnteredData()
    {
        if (_listEdges == null && _listEdges.Count == 0)
            throw new Exception("Необходимо настроить pathFinder и testEdges в инспекторе.");

        if (_startPoint == _endPoint)
            throw new Exception("StartPoint и EndPoint равны.");

        for (int i = 0; i < _listEdges.Count(); i++)
        {
            ChecingRectangle(_listEdges[i].First);
            ChecingRectangle(_listEdges[i].Second);
            if ((_listEdges.Count() - 1) != i)
                CheckeQualityRectangles(_listEdges[i + 1].First, _listEdges[i].Second);

            CheckingEdge(_listEdges[i]);
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
        
        if (CheckLateralPositioningEdge(edge))
        {
            if (edge.Start.y == edge.End.y || (edge.End.y - edge.Start.y) < 0.5f)
                throw new Exception("edge.Start равен edge.End или промежуток между ними меньше 0.5");
        }
        else if (edge.Start.x == edge.End.x || (edge.End.x - edge.Start.x) < 0.5f)
            throw new Exception("edge.Start равен edge.End или промежуток между ними меньше 0.5");
    }

    public IEnumerable<Vector2> GetPath(Vector2 start, Vector2 end, IEnumerable<Edge> edges)
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

        if (path == null || path.Count() == 0)
        {
            Debug.LogError("Путь не найден.");
            return new List<Vector2>();
        }

        path = path.Prepend(start);
        path = path.Append(end);

        return path;
    }

    private List<Vector2> GetPoints(int indexEdge, Vector2 startPoint)
    {
        Vector2 middleNextEdgePoint;
        Vector2 middle;
        Vector2 point;
        Vector2 middleEdgePoint = GetMiddlePointEdge(_listEdges[indexEdge], out float middleEdge);

        if (CheckLateralPositioningEdge(_listEdges[indexEdge + 1]))
        {
            middleNextEdgePoint = GetMiddlePointEdge(_listEdges[indexEdge + 1], out float middleTmp);
            middle = new Vector2(0, middleTmp);
            point = CalculatePointOnRay(startPoint, middleEdgePoint, middle);
        }
        else
        {
            middle = GetMiddlePointRectangle(_listEdges[indexEdge].Second);
            point = CalculatePointOnRay(startPoint, middleEdgePoint, new Vector2(0, middle.y));

            if (CheckPointOutsideRectangle(point, _listEdges[indexEdge].Second))
            {
                middle = new Vector2(middle.x, 0);
                point = CalculatePointOnRay(startPoint, middleEdgePoint, middle);
            }
            else
            {
                middle = new Vector2(0, middle.y);
            }
        }
        
        return GetListPoints(point, _listEdges[indexEdge], middleEdgePoint, middle);
    }

    private List<Vector2> GetListPoints(Vector2 point, Edge edge, Vector2 middleEdgePoint, Vector2 middle)
    {
        List<Vector2> listPoints = new();

        if (CheckPointOutsideRectangle(point, edge.Second))
        {
            point = GetMiddlePointRectangle(edge.First);
            listPoints.Add(point);
            listPoints.Add(CalculatePointOnRay(point, middleEdgePoint, middle));
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
        Vector2 point = Vector2.zero;

        point = GetMiddlePoint(edge, startPoint, middleEdgePoint, middleRectanglePoint);

        return point;
    }

    private Vector2 GetMiddlePoint(Edge edge, Vector2 startPoint, Vector2 middleEdgePoint, Vector2 middleRectanglePoint)
    {
        Vector2 point = Vector2.zero;

        if (CheckLateralPositioningEdge(edge))
        {
            point = CalculatePointOnRay(startPoint, middleEdgePoint, new Vector2(middleRectanglePoint.x, 0));
        }
        else
        {
            point = CalculatePointOnRay(startPoint, middleEdgePoint, new Vector2(0, middleRectanglePoint.y));
        }

        if (CheckPointOutsideRectangle(point, edge.Second))
        {
            point = CalculatePointOnRay(startPoint, middleEdgePoint, new Vector2(middleRectanglePoint.x, 0));
            Debug.Log(point);
        }

        return point;
    }

    private Vector2 GetMiddlePointEdge(Edge edge, out float middleEdge)
    {
        if (CheckLateralPositioningEdge(edge))
        {
            middleEdge = edge.Start.y + ((edge.End.y - edge.Start.y) / 2);
            return new Vector2(edge.Start.x, middleEdge);
        }
        else
        {
            middleEdge = edge.Start.x + ((edge.End.x - edge.Start.x) / 2);
            return new Vector2(middleEdge, edge.Start.y);
        }
    }

    private Vector2 CalculatePointOnRay(Vector2 startPoint, Vector2 endPoint, Vector2 target)
    {
        if (startPoint.x == endPoint.x)
            return new Vector2(target.x, startPoint.y);

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
            x = startPoint.x + (edge.Start.y - startPoint.y) / m;
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

    private bool CheckPointOutsideRectangle(Vector2 point, Rectangle rectangle)
    {
        float x = point.x;
        float y = point.y;
        if ((x < rectangle.Min.x) || (x > rectangle.Max.x) || (y < rectangle.Min.y) || (y > rectangle.Max.y))
            return true;
        return false;
    }
}