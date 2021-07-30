using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Rooms
{
    public class RectangleRoom //Rooms are shapes with coordinates in the GridMap space, corresponding to the gridMap[x,y]
    {

        public int x1, y1, y2, x2; //X,Y bottom left : X2,Y2 top right
        public List<Tile> RoomTiles = new List<Tile>();
        public Tile centerTile;
        public RectangleRoom(int x, int y, int width, int height)
        {
            x1 = x;
            y1 = y;
            x2 = x + width;
            y2 = y + height;
        }

        public RectangleRoom(GridMap grid, int x, int y, int width, int height)
        {
            x1 = x;
            y1 = y;
            x2 = x + width;
            y2 = y + height;

            for(int i = x1; i <= x2; ++i)
                for(int j = y1; j <= y2; ++j)
                {
                    RoomTiles.Add(grid.gameGrid[i, j]);
                }

        }
       public Vector3Int GetCenter()
        {
            return new Vector3Int( (x2 + x1) / 2, (y2 + y1) / 2, 0); 
        }

       public bool GetIntersect( RectangleRoom other )
        {
            return x1 - 1 <= other.x2 && x2 + 1 >= other.x1 && y1 - 1 <= other.y2 && y2 + 1 >= other.y1 ? true : false; //looks for intersection the additional constants guarntee that rooms will not spawn next to each other
        }
    }

    public class SquareRoom : RectangleRoom
    {
        public SquareRoom(int x, int y, int length) : base(x, y, length, length)
        { }

        public SquareRoom(GridMap grid, int x, int y, int length) : base(grid, x, y, length, length)
        { }

    }


} 