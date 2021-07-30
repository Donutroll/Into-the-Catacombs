using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAct;
public class BaseAi : MonoBehaviour
{
    public Actor parent = null;

    public string drop_item = "coin";

    void Start()
    {
        parent = GetComponent<Actor>();
        parent.ai = this;
    }

    public void get_path_to(GameManager engine, Entity self, Entity target) //creates a list of tiles towards player
    {
        Tile[,] pathMap = engine.Grid.gameGrid;

        foreach (Entity e in engine.Grid.Entities)
        {
            if (e.blockMove || e is Item)//prevents enemies from gathering by making surroundings more difficult to traverse
            {
                Tile entityTile = engine.Grid.WorldToTile(e.transform.position);
                foreach (Tile t in engine.Grid.GetNeighbouringTiles(entityTile))
                    pathMap[t.gridX, t.gridY].mCost += 30;
            }
        }

        Tile selfTile = engine.Grid.WorldToTile(self.transform.position);
        Tile targetTile = engine.Grid.WorldToTile(target.transform.position);

        List<Tile> returnPath = new PathFinding(pathMap, selfTile, targetTile).GetPath();

        if (returnPath != null)
        {
            Tile toTile = returnPath[0];
            returnPath.RemoveAt(0);
            new MovementAction(this.parent, (int)(toTile.tilePostion.x - parent.transform.position.x), (int)(toTile.tilePostion.y - parent.transform.position.y)).Perform(engine);
        }

    }

    public bool LineOfSight(Entity target)//if raycast hits a wall collider, then enemy cannot see player
    {
        RaycastHit2D hit = Physics2D.Raycast(this.transform.position, (target.transform.position - this.transform.position).normalized, (target.transform.position - this.transform.position).magnitude);
        if (hit.collider)
            return false;

        else return true;
    }

    public virtual int TakeTurn(GameManager engine)
    {
        Debug.Log("base take turn called, somethings gone wrong with: " + parent.type);
        return 0;
    }

}

    
