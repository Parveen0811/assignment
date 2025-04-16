using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public ObstacleData obstacleData;
    public GameObject obstaclePrefab;

    void Start()
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();

        foreach (Tile tile in allTiles)
        {
            Vector2Int tilePos = new Vector2Int(tile.x, tile.y);
            if (obstacleData.obstaclePositions.Contains(tilePos))
            {
                Vector3 pos = tile.transform.position + Vector3.up * 1f;
                Instantiate(obstaclePrefab, pos, Quaternion.identity, transform);
            }
        }
    }
}
