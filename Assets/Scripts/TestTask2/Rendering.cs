using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rendering : MonoBehaviour
{
    private List<Edge> _listEdges;
    private IEnumerable<Vector2> _path;

    public void Init(List<Edge> listEdges, IEnumerable<Vector2> path)
    {
        _listEdges = listEdges;
        _path = path;
    }

    private void Awake()
    {
        enabled = false;
    }

    private void Update()
    {
        for (int i = 0; i < _listEdges.Count; i++)
        {
            DrawRectangle(_listEdges[i].First);
            if (i == _listEdges.Count - 1)
                DrawRectangle(_listEdges[i].Second);
            Debug.DrawLine(_listEdges[i].Start, _listEdges[i].End, Color.green);
        }

        DrawPath(_path);
    }
    
    private void DrawRectangle(Rectangle rectangle)
    {
        Debug.DrawLine(rectangle.Min, new Vector2(rectangle.Max.x, rectangle.Min.y), Color.blue);
        Debug.DrawLine(rectangle.Min, new Vector2(rectangle.Min.x, rectangle.Max.y), Color.blue);
        Debug.DrawLine(rectangle.Max, new Vector2(rectangle.Min.x, rectangle.Max.y), Color.blue);
        Debug.DrawLine(rectangle.Max, new Vector2(rectangle.Max.x, rectangle.Min.y), Color.blue);
    }

    private void DrawPath(IEnumerable<Vector2> path)
    {
        for (int i = 0; i < path.Count() - 1; i++)
        {
            Debug.DrawLine(path.ToList()[i], path.ToList()[i + 1], Color.red);
        }
    }
}
