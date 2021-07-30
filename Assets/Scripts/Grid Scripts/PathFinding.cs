using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding 
{
    Tile[,] grid;
    int gridSizeX;
    int gridSizeY;
    public List<Tile> path;
    public Tile target;
    public Tile seeker;

    public PathFinding(Tile[,] grid, int gridSizeX, int gridSizeY, Tile seeker, Tile target)
    {
        this.grid = grid;
        this.target = target;
        this.seeker = seeker;
        this.gridSizeX = gridSizeX;
        this.gridSizeY = gridSizeY;
    }

    public PathFinding(Tile[,] grid, Tile seeker, Tile target)
    {
        this.grid = grid;
        this.target = target;
        this.seeker = seeker;
        this.gridSizeX = grid.GetLength(0);
        this.gridSizeY = grid.GetLength(1);
    }

    public List<Tile> GetPath()
    {
        FindPath();
        return path;
    }



    void FindPath()
    {
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();

        Tile startNode = seeker;
        Tile targetNode = target;
        openSet.Add(startNode);

        while(openSet.Count > 0)
        {
            Tile currentNode = openSet[0];
            for( int i = 0; i < openSet.Count; ++i) //look for cheapest node
            {
                if (openSet[i].fCost <= currentNode.fCost || openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost)
                 currentNode = openSet[i];

            }

            openSet.Remove(currentNode); //remove current node from open list
            closedSet.Add(currentNode);

            if (currentNode == targetNode) 
            {
                path = RetracePath(startNode, targetNode);
                return;
            }

            
            
            
            foreach (Tile n in GetNeighbouringNodes(currentNode)) //check the cost of neighbouring nodes   
            {
                if (!n.walkable || closedSet.Contains(n))
                    continue;
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, n) + currentNode.mCost; //additional cost for entities on spot
                
                if(newMovementCostToNeighbour < n.gCost || !openSet.Contains(n))
                {
                    n.gCost = newMovementCostToNeighbour;
                    n.hCost = GetDistance(n, targetNode);
                    n.parent = currentNode;
                    if (!openSet.Contains(n))
                        openSet.Add(n);

                }
            }
        }

    }

    public List<Tile> GetNeighbouringNodes(Tile n)
    {
        List<Tile> neighbours = new List<Tile>();

        if (n.gridX >= 0 && n.gridX <= gridSizeX && n.gridY >= 0 && n.gridY < gridSizeY) //check top limits
            neighbours.Add(grid[n.gridX, n.gridY + 1]);

        if (n.gridX >= 0 && n.gridX < gridSizeX && n.gridY >= 0 && n.gridY <= gridSizeY)//check right limits
            neighbours.Add(grid[n.gridX + 1, n.gridY]);

        if (n.gridX > 0 && n.gridX < gridSizeX && n.gridY >= 0 && n.gridY <= gridSizeY)//check left limits
            neighbours.Add(grid[n.gridX - 1, n.gridY]);

        if (n.gridX >= 0 && n.gridX < gridSizeX && n.gridY > 0 && n.gridY <= gridSizeY)//check bottom limits
            neighbours.Add(grid[n.gridX, n.gridY - 1]);

        return neighbours;
    }//check through node array


    List<Tile> RetracePath(Tile startNode, Tile endNode)
    {
        List<Tile> path = new List<Tile>();
        Tile currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        if(path == null)
            Debug.Log("Error path");

        return path;
    }


    int GetDistance(Tile nodeA, Tile nodeB)
    {
        int disX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int disY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        return 10 * (disX + disY); 
    }

}
