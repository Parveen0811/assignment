using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleData", menuName = "Grid/Obstacle Data")]
public class ObstacleData : ScriptableObject
{
    public List<Vector2Int> obstaclePositions = new List<Vector2Int>();

    public bool IsTileWalkable(Vector2Int tileCoords)
    {
        //check if the tile is an obstacle
        return !obstaclePositions.Contains(tileCoords);
    }
}

