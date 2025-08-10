using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : GridMover
{
    public Enemy enemy;
    public bool isMoving = false;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        animator.SetBool("isMoving", isMoving);
    }

    public void MoveTo(Vector2Int targetCoords, Vector2Int enemyPosition)
    {
        if (isMoving) return;

        // Temporarily treat enemy as obstacle
        obstacleData.obstaclePositions.Add(enemyPosition);

        List<Vector2Int> path = FindPath(GetCurrentGridPosition(), targetCoords);

        obstacleData.obstaclePositions.Remove(enemyPosition);

        if (path != null && path.Count > 0 && !enemy.isMoving)
        {
            StartCoroutine(FollowPath(path));
        }
        else
        {
            Debug.Log("No valid path found!");
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

        // After player finishes moving, trigger enemy move
        Enemy enemy = FindObjectOfType<Enemy>();
        if (enemy != null)
        {
            enemy.MoveToNearestNeighbor(GetCurrentGridPosition());
        }
    }

    public Vector2Int GetCurrentGridPosition()
    {
        return new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.z));
    }
}
