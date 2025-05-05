using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    private bool[,] tiles;
    private readonly List<Node> openList = new();
    private readonly HashSet<Node> closedList = new();

    public void SetTiles(bool[,] tiles)
    {
        this.tiles = tiles;
    }

    public List<Node> Find(Vector2Int start, Vector2Int end)
    {
        Node startNode = new(false, start.x, start.y);
        Node endNode = new(false, end.x, end.y);

        openList.Clear();
        closedList.Clear();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = GetNodeWithLowestF(openList);

            if (currentNode.X == endNode.X && currentNode.Y == endNode.Y)
            {
                return RetracePath(currentNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (Node neighbor in GetNeighbors(currentNode))
            {
                if (closedList.Contains(neighbor) || neighbor.IsWall)
                {
                    continue;
                }

                int newG = currentNode.G + 1;

                if (!openList.Contains(neighbor) || newG < neighbor.G)
                {
                    neighbor.G = newG;
                    neighbor.H = Mathf.Abs(neighbor.X - endNode.X) + Mathf.Abs(neighbor.Y - endNode.Y);
                    neighbor.Parent = currentNode;

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return new List<Node>();
    }

    private Node GetNodeWithLowestF(List<Node> openList)
    {
        var lowestFNode = openList[0];
        foreach (var node in openList)
        {
            if (node.F < lowestFNode.F)
            {
                lowestFNode = node;
            }
        }
        return lowestFNode;
    }

    private List<Node> GetNeighbors(Node currentNode)
    {
        List<Node> neighbors = new();

        foreach (var direction in Direction.VECTORS)
        {
            int x = currentNode.X + direction.x;
            int y = currentNode.Y + direction.y;

            // 경계 밖이면 무시
            if (x < 0 || y < 0 || x >= tiles.GetLength(0) || y >= tiles.GetLength(1))
            {
                continue;
            }

            neighbors.Add(new Node(tiles[x, y], x, y));
        }

        return neighbors;
    }

    private List<Node> RetracePath(Node endNode)
    {
        List<Node> path = new();
        var currentNode = endNode;

        // 경로를 역추적
        while (currentNode != null)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        // 경로를 시작점에서 끝점 순으로 뒤집기
        path.Reverse();
        return path;
    }

    public void EnqueueRouteMovementValue(Vector2Int start, Vector2Int end, Queue<Vector2Int> route)
    {
        List<Node> routeNodes = Find(start, end);
        for (int index = 1; index < routeNodes.Count; index++)
        {
            int x = routeNodes[index].X - routeNodes[index - 1].X;
            int y = routeNodes[index].Y - routeNodes[index - 1].Y;
            route.Enqueue(new Vector2Int(x, -y));
        }
    }
}