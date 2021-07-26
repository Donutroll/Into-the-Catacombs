using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
/*
public class CreateGrid : MonoBehaviour
{

    public GameObject player;


    public Vector3 gridWorldSize;
    public Tilemap unwalkable;
    public Tile[,] grid;

    public int gridSizeX, gridSizeY;
    public float nodeRadius;
    public float nodeDiameter;
    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        GridCreate();
    }

    private void GridCreate() //checks walkable against tilemap settings
    {
        grid = new Tile[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * (gridSizeX / 2) - Vector3.up * (gridSizeY / 2);
        for(int x = 0; x < gridSizeX; ++x)
        {
            for(int y = 0; y < gridSizeY; ++y)
            {
                Vector3 worldPosition = worldBottomLeft + new Vector3(x * nodeDiameter + nodeRadius, y * nodeDiameter + nodeRadius, 0);
                Tile tile = new Tile(false, worldPosition, x, y);
                grid[x, y] = tile;

                if (unwalkable.HasTile(unwalkable.WorldToCell(grid[x, y].worldPos)))
                    grid[x, y].walkable = false;
                else
                    grid[x, y].walkable = true;


            }
        }
    }
    
    public Tile GetNodeFromWorldPosition(Vector3 worldPosition) //could be done by calculating (vector.x - gizmo.x distance from origin) / gridworldsize
    {
        int x = Mathf.RoundToInt(worldPosition.x - 0.5f + (gridWorldSize.x/2) ); //only accounts for when grid position 0,0,0 (-0.5 for offset when rounding up at some number w/ .5)
        int y = Mathf.RoundToInt(worldPosition.y - 0.5f + (gridWorldSize.y/2) );
        return grid[x, y];
    }
    public List<Tile> GetNeighbouringNodes(Tile n)
    {
        List<Tile> neighbours = new List<Tile>();

        if (n.gridX >= 0 && n.gridX <= gridSizeX && n.gridY >= 0 && n.gridY < gridSizeY) //check top
            neighbours.Add(grid[n.gridX, n.gridY + 1]);

        if(n.gridX >=0 && n.gridX < gridSizeX && n.gridY >= 0 && n.gridY <= gridSizeY)//check right
            neighbours.Add(grid[n.gridX + 1, n.gridY]);

        if (n.gridX > 0 && n.gridX < gridSizeX && n.gridY >= 0 && n.gridY <= gridSizeY)//check left
            neighbours.Add(grid[n.gridX -1, n.gridY]);

        if (n.gridX >= 0 && n.gridX < gridSizeX && n.gridY > 0 && n.gridY <= gridSizeY)//check bottom
            neighbours.Add(grid[n.gridX, n.gridY - 1]);


        return neighbours;
    }//check through node array


    public List<Tile> path;

    private void OnDrawGizmos() 
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 1));

        if(grid != null)
        {
            Tile playerNode = GetNodeFromWorldPosition(player.transform.position);

            foreach( Tile n in grid)
            {
                if (n != null)
                {
                   
                    Gizmos.color = n.walkable ? Color.white : Color.red;

                    if (playerNode == n)
                        Gizmos.color = Color.green;
                    
                    if(path != null)
                    {
                        if (path.Contains(n))
                            Gizmos.color = Color.cyan;
                    }
                        
                    Gizmos.DrawCube(n.worldPos, Vector3.one * (nodeRadius));
                }
            }
        }
    } 
}
 */