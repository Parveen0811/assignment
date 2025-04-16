using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;
    public ObstacleData obstacleData;
    public int gridWidth = 10;
    public int gridHeight = 10;

    private Vector2Int enemyPosition;

    void Start()
    {
        enemyPosition = WorldToGrid(transform.position);
    }

    public void MoveToNearestNeighbor(Vector2Int clickedTile)
    {
        // Step 1: Get valid neighbors of the clicked tile
        List<Vector2Int> neighbors = GetNeighbors(clickedTile);
        neighbors.RemoveAll(tile => !obstacleData.IsTileWalkable(tile)); // Remove non-walkable tiles

        if (neighbors.Count == 0)
        {
            Debug.Log("No valid neighbors to move to.");
            return;
        }

        // Step 2: Find the neighbor closest to the enemy
        Vector2Int closestTile = neighbors[0];
        float closestDistance = Vector2.Distance(GridToWorld(enemyPosition), GridToWorld(neighbors[0]));

        foreach (Vector2Int neighbor in neighbors)
        {
            float distance = Vector2.Distance(GridToWorld(enemyPosition), GridToWorld(neighbor));
            if (distance < closestDistance)
            {
                closestTile = neighbor;
                closestDistance = distance;
            }
        }

        // Step 3: Generate a path to the closest tile
        List<Vector2Int> path = FindPath(enemyPosition, closestTile);

        if (path != null && path.Count > 0)
        {
            // Step 4: Follow the generated path
            StopAllCoroutines();
            StartCoroutine(FollowPath(path));
            enemyPosition = closestTile; // Update enemy's position
        }
        else
        {
            Debug.Log("No valid path to the target tile.");
        }
    }

    private IEnumerator FollowPath(List<Vector2Int> path)
    {
        foreach (Vector2Int tile in path)
        {
            Vector3 targetPosition = new Vector3(tile.x, transform.position.y, tile.y);

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            // Snap to the exact position to avoid small discrepancies
            transform.position = targetPosition;
        }
    }

    private List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        // Nodes for A* algorithm
        HashSet<Vector2Int> openSet = new HashSet<Vector2Int>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int>();

        // Initialize scores
        openSet.Add(start);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, target);

        while (openSet.Count > 0)
        {
            // Get the node with the lowest fScore
            Vector2Int current = GetLowestFScoreNode(openSet, fScore);

            if (current == target)
            {
                // Reconstruct the path
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            // Check neighbors
            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                if (closedSet.Contains(neighbor) || !obstacleData.IsTileWalkable(neighbor))
                {
                    continue;
                }

                int tentativeGScore = gScore[current] + 1;

                if (!openSet.Contains(neighbor))
                {
                    openSet.Add(neighbor);
                }
                else if (tentativeGScore >= gScore[neighbor])
                {
                    continue;
                }

                // Update scores and path
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, target);
            }
        }

        // No path found
        return null;
    }

    private List<Vector2Int> GetNeighbors(Vector2Int node)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Add possible neighbors (up, down, left, right)
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbor = node + direction;

            // Check if the neighbor is within grid bounds
            if (neighbor.x >= 0 && neighbor.x < gridWidth && neighbor.y >= 0 && neighbor.y < gridHeight)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y); // Manhattan distance
    }

    private Vector2Int GetLowestFScoreNode(HashSet<Vector2Int> openSet, Dictionary<Vector2Int, int> fScore)
    {
        Vector2Int lowestNode = default;
        int lowestScore = int.MaxValue;

        foreach (Vector2Int node in openSet)
        {
            if (fScore.TryGetValue(node, out int score) && score < lowestScore)
            {
                lowestScore = score;
                lowestNode = node;
            }
        }

        return lowestNode;
    }

    private List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2Int> path = new List<Vector2Int> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, transform.position.y, gridPosition.y);
    }
}
