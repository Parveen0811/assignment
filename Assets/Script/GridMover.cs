using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class GridMover : MonoBehaviour
{
    public float moveSpeed = 5f;
    public ObstacleData obstacleData;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public bool isMoving = false;
    protected Animator animator;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        if (animator != null)
            animator.SetBool("isMoving", isMoving);
    }

    protected IEnumerator FollowPath(List<Vector2Int> path)
    {
        isMoving = true;

        foreach (Vector2Int tile in path)
        {
            Vector3 targetPosition = new Vector3(tile.x, transform.position.y, tile.y);

            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);

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
    }

    protected List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        HashSet<Vector2Int> openSet = new HashSet<Vector2Int>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int>();
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int>();

        openSet.Add(start);
        gScore[start] = 0;
        fScore[start] = Heuristic(start, target);

        while (openSet.Count > 0)
        {
            Vector2Int current = GetLowestFScoreNode(openSet, fScore);

            if (current == target)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

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
                else if (tentativeGScore >= gScore.GetValueOrDefault(neighbor, int.MaxValue))
                {
                    continue;
                }

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, target);
            }
        }

        return null;
    }

    protected List<Vector2Int> GetNeighbors(Vector2Int node)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbor = node + direction;
            if (neighbor.x >= 0 && neighbor.x < gridWidth && neighbor.y >= 0 && neighbor.y < gridHeight)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    protected int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    protected Vector2Int GetLowestFScoreNode(HashSet<Vector2Int> openSet, Dictionary<Vector2Int, int> fScore)
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

    protected List<Vector2Int> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
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
}
