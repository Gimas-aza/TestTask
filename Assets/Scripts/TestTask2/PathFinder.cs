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
                Debug.Log(point);
            }
        }
        else
        {
            Debug.LogError("Путь не найден.");
        }
    }

    public IEnumerable<Vector2> GetPath(Vector2 A, Vector2 C, IEnumerable<Edge> edges)
    {
        // Создаем список для хранения узлов, которые нужно посетить
        var openSet = new List<Node>();

        // Создаем начальный узел и добавляем его в открытый список
        var startNode = new Node(A, null, 0);
        openSet.Add(startNode);

        // Создаем словарь для хранения стоимости пути от начального узла до текущего узла
        var gScores = new Dictionary<Node, float>();
        gScores[startNode] = 0;

        // Начинаем поиск пути
        while (openSet.Count > 0)
        {
            // Находим узел с наименьшей стоимостью из открытого списка
            var currentNode = openSet.OrderBy(node => node.Cost).First();
        
            // Если мы достигли конечной точки, восстанавливаем путь и возвращаем его
            if (currentNode.Position == C)
            {
                return ReconstructPath(currentNode);
            }

            // Удаляем текущий узел из открытого списка
            openSet.Remove(currentNode);

            // Рассматриваем соседние узлы
            foreach (var edge in edges)
            {
                // if (edge.First.Equals(edge.Second)) continue; // Пропускаем рёбра, соединяющие один и тот же прямоугольник

                // Проверяем, если текущий узел находится на одном из концов ребра
                if (currentNode.Position == (Vector2) edge.Start || currentNode.Position == (Vector2) edge.End)
                {
                    Debug.Log("currentNode: " + currentNode.Position);
                    // Вычисляем следующий узел на ребре
                    var nextPosition = (currentNode.Position == (Vector2) edge.Start) ? edge.End : edge.Start;

                    // Рассчитываем стоимость перемещения на ребро
                    var movementCost = Vector2.Distance(currentNode.Position, nextPosition);

                    // Учитываем повороты
                    if (currentNode.Previous != null)
                    {
                        var prevDirection = currentNode.Position - currentNode.Previous.Position;
                        var nextDirection = (Vector2) nextPosition - currentNode.Position;
                        if (Vector2.Dot(prevDirection.normalized, nextDirection.normalized) < 0)
                        {
                            movementCost += 1; // Увеличиваем стоимость на поворот
                        }
                    }

                    // Вычисляем общую стоимость для следующего узла
                    var tentativeGScore = gScores[currentNode] + movementCost;

                    // Если следующий узел еще не в открытом списке или новая стоимость меньше старой
                    if (!openSet.Any(node => node.Position == (Vector2) nextPosition) || 
                        tentativeGScore < gScores[new Node(nextPosition, currentNode, tentativeGScore)])
                    {
                        // Создаем новый узел
                        var nextNode = new Node(nextPosition, currentNode, tentativeGScore);
                        openSet.Add(nextNode);
                        gScores[nextNode] = tentativeGScore;
                    }
                }
            }
        }

        // Если не удалось найти путь, возвращаем null
        return null;
    }

    // Метод для восстановления пути из узлов
    private IEnumerable<Vector2> ReconstructPath(Node node)
    {
        var path = new List<Vector2>();
        while (node != null)
        {
            path.Add(node.Position);
            node = node.Previous;
        }
        path.Reverse();
        return path;
    }

    private class Node
    {
        public Vector2 Position { get; }
        public Node Previous { get; }
        public float Cost { get; }

        public Node(Vector2 position, Node previous, float cost)
        {
            Position = position;
            Previous = previous;
            Cost = cost;
        }
    }
}
