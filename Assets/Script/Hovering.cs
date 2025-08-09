using UnityEngine;
using UnityEngine.UI;


public class Hovering : MonoBehaviour
{
    public LayerMask tileLayer;
    public Text positionText;
    public ObstacleData obstacleData;
    public Player player;
    public Enemy enemy;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, tileLayer))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                // Display the tile's x and y coordinates
                positionText.text = $"X: {tile.x}, Y: {tile.y}";

                // Check for mouse click
                if (Input.GetMouseButtonDown(0) )
                {
                    Vector2Int tileCoords = new Vector2Int(tile.x, tile.y);

                    // Check if the tile is walkable
                    if (obstacleData.IsTileWalkable(tileCoords) && !player.isMoving)
                    {
                        player.MoveTo(tileCoords, enemy.enemyPosition);
                        enemy.MoveToNearestNeighbor(tileCoords);
                    }
                    else
                    {
                        Debug.Log("Tile is not walkable!");
                    }
                }
            }
        }
        else
        {
            positionText.text = "No tile hovered";
        }
    }
}
