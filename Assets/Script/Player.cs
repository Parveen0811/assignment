using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

public class Player : GridMover
{
    public void MoveTo(Vector2Int targetCoords, Vector2Int enemyPosition)
    {
        if (isMoving) return;

        obstacleData.obstaclePositions.Add(enemyPosition);

        // Use base class pathfinding
        List<Vector2Int> path = FindPath(
            new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z)),
            targetCoords
        );

        obstacleData.obstaclePositions.Remove(enemyPosition);

        if (path != null && path.Count > 0)
        {
            StopAllCoroutines();
            StartCoroutine(FollowPath(path));
        }
        else
        {
            Debug.Log("No valid path found!");
        }
    }
}
