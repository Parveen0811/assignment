using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : GridMover
{
    public bool isMoving = false;
    public Player player;
    public Vector2Int enemyPosition;
    private Animator animator;

    private void Start()
    {
        enemyPosition = WorldToGrid(transform.position);
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("isMoving", isMoving);
    }
    // Converts a world position to a grid position (assuming 1 unit per grid cell)
    private Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.z));
    }

    public void MoveToNearestNeighbor(Vector2Int targetTile)
    {
        if (isMoving) return;

        // Get valid neighbors of the target tile
        List<Vector2Int> neighbors = GetNeighbors(targetTile);
        neighbors.RemoveAll(tile => !obstacleData.IsTileWalkable(tile));

        if (neighbors.Count == 0)
        {
            Debug.Log("No valid neighbors to move to.");
            return;
        }

        // Find the neighbor closest to the enemy's current position
        Vector2Int currentPos = GetCurrentGridPosition();
        Vector2Int closestTile = neighbors[0];
        float closestDistance = Vector2.Distance((Vector2)currentPos, (Vector2)neighbors[0]);

        foreach (Vector2Int neighbor in neighbors)
        {
            float distance = Vector2.Distance((Vector2)currentPos, (Vector2)neighbor);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTile = neighbor;
            }
        }

        List<Vector2Int> path = FindPath(currentPos, closestTile);

        if (path != null && path.Count > 0 && !player.isMoving)
        {
            StartCoroutine(FollowPath(path));
        }
        else
        {
            Debug.Log("No valid path to the target tile.");
        }
    }

    private IEnumerator FollowPath(List<Vector2Int> path)
    {
        isMoving = true;

        foreach (Vector2Int tile in path)
        {
            Vector3 targetPosition = new Vector3(tile.x, transform.position.y, tile.y);

            // Calculate direction and target rotation
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                // Smoothly rotate towards the target direction
                while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
                    yield return null;
                }
                transform.rotation = targetRotation;
            }

            while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
            transform.position = targetPosition;
        }

        isMoving = false;
        // Now player can move again (input is enabled by isMoving checks)
    }

    public Vector2Int GetCurrentGridPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    }
}
