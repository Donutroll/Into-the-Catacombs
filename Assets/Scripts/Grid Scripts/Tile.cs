using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile //tile world positions are described by bottom left
{
    public Tile()
    { }
    public Tile(Vector3 position, bool walkable)
    {
        this.position = position;
        this.walkable = walkable;
    }

    public Tile(Vector3 position, bool walkable, int gridX, int gridY)
    {
        this.position = position;
        this.walkable = walkable;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public bool walkable = true;
    public Vector3 position;
    public int gridX, gridY;
    public Vector3 tilePostion //instantiated after position, returns middle of tile in world 
    {

        get
        {
            return position + new Vector3(0.5f, 0.5f, 0);
        }

    }

    public int hCost, gCost; //for pathfinding
    public int mCost = 0;
    public List<Tile> neighbourNodes;
    public Tile parent;
    public int fCost
    {
        get
        {
            return hCost + gCost;
        }
    }
}
