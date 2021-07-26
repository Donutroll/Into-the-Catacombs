using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public Vector3 worldPos; //position in world
    public int gridX, gridY; //position in 2d array 
    public bool walkable = false;
    public List<Node> neighbourNodes;
    public Node parent;

    public int hCost, gCost; //g is cost from start to n, h is cost from n to end 

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPos = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost //total cost
    {
        get
        {
            return hCost + gCost;
        }
    }
}
