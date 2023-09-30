using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour, IPathFinder
{
    [SerializeField] private Vector2 _startPoint;
    [SerializeField] private Vector2 _endPoint;
    [SerializeField] private List<Edge> _listEdges;

    private InputValidator _inputValidator;
    private IEnumerable<Vector2> _path = new List<Vector2>();
    private Rendering _rendering;

    private void Start()
    {
        _inputValidator = new InputValidator();
        _inputValidator.CheckingEnteredData(_startPoint, _endPoint, _listEdges);

        _path = GetPath(_startPoint, _endPoint, _listEdges);

        if (TryGetComponent(out _rendering))
        {
            _rendering.Init(_listEdges, _path);
            _rendering.enabled = true;
        }
    }

    public IEnumerable<Vector2> GetPath(Vector2 start, Vector2 end, IEnumerable<Edge> edges)
    {
        List<Vector2> path = new();
        Vector2 lastPoint = start;

        for (int i = 0; i < edges.Count() - 1; i++)
        {
            List<Vector2> points = GetPoints(i, lastPoint);
            path.AddRange(points);
            lastPoint = points.Last();
        }

        Vector2 pointOnSegment = CalculatePointOnSegment(lastPoint, end, edges.Last());
        if (!CheckPointPassingOverEdge(pointOnSegment, edges.Last()))
        {
            lastPoint = GetLastPoint(edges.Last(), lastPoint);
            path.Add(lastPoint);
        }

        if (path == null || path.Count() == 0)
        {
            Debug.LogError("Путь не найден.");
            return Enumerable.Empty<Vector2>();
        }

        path.Insert(0, start);
        path.Add(end);

        return path;
    }

    private List<Vector2> GetPoints(int indexEdge, Vector2 startPoint)
    {
        Vector2 middleNextEdgePoint;
        Vector2 middleOffset;
        Vector2 point;
        Vector2 middleEdgePoint = GetMiddlePointEdge(_listEdges[indexEdge], out float middleEdge);

        if (CheckLateralPositioningEdge(_listEdges[indexEdge + 1]))
        {
            middleNextEdgePoint = GetMiddlePointEdge(_listEdges[indexEdge + 1], out float middleTmp);
            middleOffset = new Vector2(0, middleTmp);
            point = CalculatePointOnRay(startPoint, middleEdgePoint, middleOffset);
        }
        else
        {
            middleOffset = GetMiddlePointRectangle(_listEdges[indexEdge].Second);
            point = CalculatePointOnRay(startPoint, middleEdgePoint, new Vector2(0, middleOffset.y));

            if (CheckPointOutsideRectangle(point, _listEdges[indexEdge].Second))
            {
                middleOffset = new Vector2(middleOffset.x, 0);
                point = CalculatePointOnRay(startPoint, middleEdgePoint, middleOffset);
            }
            else
            {
                middleOffset = new Vector2(0, middleOffset.y);
            }
        }
        
        return GetListPoints(point, _listEdges[indexEdge], middleEdgePoint, middleOffset);
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

        point = CalculatePointOnRay(startPoint, middleEdgePoint, CheckLateralPositioningEdge(edge)
            ? new Vector2(middleRectanglePoint.x, 0)
            : new Vector2(0, middleRectanglePoint.y));

        if (CheckPointOutsideRectangle(point, edge.Second))
        {
            point = CalculatePointOnRay(startPoint, middleEdgePoint, new Vector2(middleRectanglePoint.x, 0));
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
        return edge.Start.x == edge.End.x;
    }

    private bool CheckPointPassingOverEdge(Vector2 point, Edge edge)
    {
        return (CheckLateralPositioningEdge(edge) && point.y > edge.Start.y && point.y < edge.End.y)
            || (point.x > edge.Start.x && point.x < edge.End.x);
    }

    private bool CheckPointOutsideRectangle(Vector2 point, Rectangle rectangle)
    {
        return point.x < rectangle.Min.x || point.x > rectangle.Max.x 
            || point.y < rectangle.Min.y || point.y > rectangle.Max.y;
    }
}