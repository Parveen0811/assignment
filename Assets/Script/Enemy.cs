using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : GridMover
{
    private Vector2Int enemyPosition;

    protected override void Start()
    {
        base.Start();
        enemyPosition = WorldToGrid(transform.position);
    }

    // Converts world position to grid coordinates
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    // Converts grid coordinates to world position
    private Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, transform.position.y, gridPosition.y);
    }

    public void MoveToNearestNeighbor(Vector2Int targetTile)
    {
        // 1. Get valid neighbors of the target tile
        List<Vector2Int> neighbors = GetNeighbors(targetTile);
        neighbors.RemoveAll(tile => !obstacleData.IsTileWalkable(tile));

        if (neighbors.Count == 0)
        {
            Debug.Log("No valid neighbors to move to.");
            return;
        }

        // 2. Find the neighbor closest to the enemy's current position
        Vector2Int closestTile = neighbors[0];
        float closestDistance = Vector2.Distance(GridToWorld(enemyPosition), GridToWorld(neighbors[0]));

        foreach (Vector2Int neighbor in neighbors)
        {
            float distance = Vector2.Distance(GridToWorld(enemyPosition), GridToWorld(neighbor));
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = neighbor;
            }
        }

        // 3. Generate a path to the closest tile
        List<Vector2Int> path = FindPath(enemyPosition, closestTile);

        if (path != null && path.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(FollowPath(path));
            enemyPosition = closestTile;
        }
        else
        {
            Debug.Log("No valid path to the target tile.");
        }
    }
}
