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

        Debug.Log(_path.Count());

        if (_path != null)
        {
            Debug.Log("Найденный путь:");
            foreach (Vector2 point in _path)
            {
                Debug.Log(point.x + " " + point.y);
            }
        }
        else
        {
            Debug.LogError("Путь не найден.");
        }
    }

    private void Update()
    {
        for (int i = 0; i < _path.Count() - 1; i++)
        {
            Debug.DrawLine(_path.ToList()[i], _path.ToList()[i + 1], Color.red);
        }
    }

    private IEnumerable<Vector2> GetPath(Vector2 start, Vector2 end, IEnumerable<Edge> edges)
    {
        IEnumerable<Vector2> path = new List<Vector2>();
        Vector2 lastPoint = start;

        for (int i = 0; i < edges.Count() - 1; i++)
        {
            Debug.Log(i);
            if (CheckStartPoint(edges.ToList()[i], lastPoint))
            {
                lastPoint = GetPoint(i, lastPoint);
                path = path.Append(lastPoint);
            }
            else
            {

            }
        }

        return path;
    }

    private bool CheckStartPoint(Edge edge, Vector2 startPoint)
    {
        if (edge.Start.x == edge.End.x && startPoint.x == edge.Start.x) return false;
        else if (edge.Start.y == edge.End.y && startPoint.y == edge.Start.y) return false;
        
        return true;
    }

    private Vector2 GetPoint(int indexEdge, Vector2 startPoint)
    {
        Vector2 middleEdgePoint = GetMiddlePointEdge(testEdges[indexEdge], out float middleEdge);
        Vector2 middleNextEdgePoint = new Vector2();
        float middleEdgeNext = 0;

        if (testEdges.Count() - 1 == indexEdge)
            middleNextEdgePoint = endPoint;
        else
            middleNextEdgePoint = GetMiddlePointEdge(testEdges[indexEdge + 1], out middleEdgeNext);
        
        Vector2 result = CalculatePointOnRay(startPoint, middleEdgePoint, middleEdgeNext);
        return result;
    }

    private Vector2 GetMiddlePointEdge(Edge edge, out float middleEdge)
    {
        if (edge.Start.x == edge.End.x)
        {
            middleEdge = edge.Start.y + ((edge.End.y - edge.Start.y) / 2);
            return new Vector2(edge.Start.x, middleEdge);
        }
        else if (edge.Start.y == edge.End.y)
        {
            middleEdge = edge.Start.x + ((edge.End.x - edge.Start.x) / 2);
            return new Vector2(middleEdge, edge.Start.y);
        }

        middleEdge = 0;
        return Vector2.zero;
    }

    private Vector2 CalculatePointOnRay(Vector2 startPoint, Vector2 endPoint, float targetY)
    {
        float m = (float) (endPoint.y - startPoint.y) / (endPoint.x - startPoint.x);
        float x = startPoint.x + (targetY - startPoint.y) / m;

        return new Vector2(x, targetY);
    }
}