using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder : MonoBehaviour, IPathFinder
{
    public Vector2 startPoint; // Начальная точка A
    public Vector2 endPoint; // Конечная точка C
    public List<Edge> testEdges; // Список тестовых рёбер

    void Start()
    {
        // Проверяем, что pathFinder и testEdges установлены
        if (testEdges == null)
        {
            Debug.LogError("Необходимо настроить pathFinder и testEdges в инспекторе.");
            return;
        }

        // Вызываем метод GetPath вашего pathFinder
        IEnumerable<Vector2> path = GetPath(startPoint, endPoint, testEdges);

        // Проверяем результат и выводим его
        if (path != null)
        {
            Debug.Log("Найденный путь:");
            foreach (Vector2 point in path)
            {
                Debug.Log(point.x);
            }
        }
        else
        {
            Debug.LogError("Путь не найден.");
        }
    }

   public IEnumerable<Vector2> GetPath(Vector2 start, Vector2 end, IEnumerable<Edge> edges)
    {
        // Создаём граф, представляющий рёбра и их вершины
        Dictionary<Vector2, List<Vector2>> graph = CreateGraph(edges);

        // Используем алгоритм поиска пути, например, A*
        List<Vector2> path = AStarSearch(graph, start, end);

        return path;
    }

    private Dictionary<Vector2, List<Vector2>> CreateGraph(IEnumerable<Edge> edges)
    {
        Dictionary<Vector2, List<Vector2>> graph = new Dictionary<Vector2, List<Vector2>>();

        foreach (Edge edge in edges)
        {
            if (!graph.ContainsKey(edge.Start))
            {
                graph[edge.Start] = new List<Vector2>();
            }

            if (!graph.ContainsKey(edge.End))
            {
                graph[edge.End] = new List<Vector2>();
            }

            graph[edge.Start].Add(edge.End);
            graph[edge.End].Add(edge.Start);
        }

        return graph;
    }

    private List<Vector2> AStarSearch(Dictionary<Vector2, List<Vector2>> graph, Vector2 start, Vector2 end)
    {
        List<Vector2> path = new List<Vector2>();
        PriorityQueue<Vector2> openSet = new PriorityQueue<Vector2>();
        Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
        Dictionary<Vector2, float> gScore = new Dictionary<Vector2, float>();

        openSet.Enqueue(start, 0);
        gScore[start] = 0;

        while (openSet.Count > 0)
        {
            Vector2 current = openSet.Dequeue();

            if (current == end)
            {
                path = ReconstructPath(cameFrom, current);
                break;
            }

            foreach (Vector2 neighbor in graph[current])
            {
                float tentativeGScore = gScore[current] + Vector2.Distance(current, neighbor);
                
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {

                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    float priority = tentativeGScore + Vector2.Distance(neighbor, end);
                    openSet.Enqueue(neighbor, priority);
                }
            }
        }

        return path;
    }

    private List<Vector2> ReconstructPath(Dictionary<Vector2, Vector2> cameFrom, Vector2 current)
    {
        List<Vector2> path = new List<Vector2>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(current);
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }
}

// PriorityQueue для использования в A*
public class PriorityQueue<T>
{
    private List<(T, float)> elements = new List<(T, float)>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
    }

    public T Dequeue()
    {
        int bestIndex = 0;

        for (int i = 0; i < elements.Count; i++)
        {
            if (elements[i].Item2 < elements[bestIndex].Item2)
            {
                bestIndex = i;
            }
        }

        T bestItem = elements[bestIndex].Item1;
        elements.RemoveAt(bestIndex);
        return bestItem;
    }     
}
