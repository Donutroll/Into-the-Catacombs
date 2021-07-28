using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightingRenderer : MonoBehaviour
{
    public GameManager engine;
    public GridMap Grid;
    public GameObject player;

    public Tilemap visibilityMap;
    public TileBase dark_Sprite;
    public int playerLightRadius;
    private void Start()
    {
        Grid = engine.Grid;
        player = engine.player;
        DrawFog();
    }


    public void RenderLight(int rad, Entity entity)
    {
        Vector3 entityPos = entity.transform.position;
        Tile playerTile = Grid.WorldToTile(entityPos);
        int radius = rad;
        Rooms.RectangleRoom viewSpace = new Rooms.RectangleRoom(playerTile.gridX - radius, playerTile.gridY - radius, radius * 2, radius * 2);

        for (int x = viewSpace.x1 + 1; x < viewSpace.x2; ++x)//foreach tile in the player's radius, excluding corners
            for (int y = viewSpace.y1 + 1; y < viewSpace.y2; ++y)
            {
                Vector3Int cellPosLocal = visibilityMap.WorldToCell(Grid.gameGrid[x, y].tilePostion);
                Vector3 cellPosWorld = visibilityMap.CellToWorld(cellPosLocal);
                if (visibilityMap.HasTile(cellPosLocal))
                {
                    float x_dir = cellPosWorld.x < entityPos.x ? 1 : -1;
                    float y_dir = cellPosWorld.y < entityPos.y ? 1 : -1;
                    Vector3 toCellPoint = cellPosWorld + new Vector3(0.5f, 0.5f, 0) + new Vector3(x_dir, y_dir, 0) / 2f; //find the closest corner

                    RaycastHit2D hit = Physics2D.Raycast(entityPos, (toCellPoint - entityPos).normalized, Mathf.Abs(Vector3.Distance(entityPos, toCellPoint))); //cast a ray to corner
                    if (!hit.collider || Mathf.Abs(Vector3.Distance(hit.point, toCellPoint)) < Mathf.Epsilon) //if that tile has no collider or if the ray hits the tile corner with a degree of error
                        visibilityMap.SetTile(cellPosLocal, null);


                }
            }

        for (int x = 0; x < GridMap.gameGrid_x; ++x)//draws fog on all tiles not in the viewSpace
            for (int y = 0; y < GridMap.gameGrid_y; ++y)
            {
                Vector3Int cellPosLocal = visibilityMap.WorldToCell(Grid.gameGrid[x, y].tilePostion);
                if (!visibilityMap.HasTile(cellPosLocal) && Mathf.Abs(Vector3.Distance(cellPosLocal, entityPos)) > radius)
                    visibilityMap.SetTile(cellPosLocal, dark_Sprite);


            }
    }
    

    public void DrawFog()
    {
        for (int x = 0; x < GridMap.gameGrid_x; ++x)
            for (int y = 0; y < GridMap.gameGrid_y; ++y)
            {
                Vector3Int cellPos = visibilityMap.WorldToCell(Grid.gameGrid[x, y].position);
                visibilityMap.SetTile(cellPos, dark_Sprite);
            }
    }


}
