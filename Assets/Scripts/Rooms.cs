using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Rooms
{
    public class RectangleRoom 
    {
        public RectangleRoom(int x, int y, int width, int height)
        {
            x1 = x;
            y1 = y;
            x2 = x + width;
            y2 = y + height;
        }
        
       public Vector3Int GetCenter()
        {
            return new Vector3Int( (x2 + x1) / 2, (y2 + y1) / 2, 0); 
        }

       public bool GetIntersect( RectangleRoom other )
        {
            return x1 <= other.x2 && x2 >= other.x1 && y1 <= other.y2 && y2 >= other.y1 ? true : false; //looks for intersection
        }


        

        public int x1, y1, y2, x2; //X,Y bottom left : X2,Y2 top right


    }


} 