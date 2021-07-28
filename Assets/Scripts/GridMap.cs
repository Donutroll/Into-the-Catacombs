using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Rooms;


public class GridMap : MonoBehaviour
{
    public const int gameGrid_x = 80;
    public const int gameGrid_y = 60;

    const int mapRangeX1 = 15;
    const int mapRangeX2 = 60;
    const int mapRangeY1 = 15;
    const int mapRangeY2 = 40;

    public int bottom_x = -(gameGrid_x / 2);
    public int bottom_y = -(gameGrid_y / 2);
    public int top_x = gameGrid_x / 2;
    public int top_y = gameGrid_y / 2;

    public Tile[,] gameGrid = new Tile[gameGrid_x, gameGrid_y];

    public List<RectangleRoom> Rooms = new List<RectangleRoom>();
    public List<Entity> Entities = new List<Entity>();
    public List<Actor> Actors//REMINDER: this is a READ ONLY property, you can change it later
    {
        get
        {
            List<Actor> Actors = new List<Actor>();
            foreach (Entity e in Entities)
            {
                if (e is Actor)
                {
                    Actor tmpE = (Actor)e;
                    if (tmpE.is_alive())
                        Actors.Add(tmpE);
                }
            }

            return Actors; //returns all Acting Entities
        }
    }
    public Tilemap floorTiles;
    public Tilemap wallTiles;
    public TileBase floor_sprite;
    public TileBase nextFloor_sprite;
    public TileBase wall_sprite;

    public Vector3 stairWorldPos;
    public Vector3Int stairGridPos;


    private void Start()
    {

    }


    /// <summary>
    ///      x * * * * *
    ///    ^ x * * * * * 
    ///  j | x x * * * *
    ///      i ->
    /// </summary>



    //Grid Generation methods

    void CreateRoom(RectangleRoom Room) //sets inner room tiles to walkable
    {
        for (int x = Room.x1 ; x <= Room.x2 ; ++x)
            for(int y = Room.y1 ; y <= Room.y2 ; ++y)
            {
                    gameGrid[x, y].walkable = true;
  
            }
    }
    
    void CreateTunnel(RectangleRoom r1, RectangleRoom r2)
    {
        Vector3Int point1 = new Vector3Int(r1.GetCenter().x, r1.GetCenter().y, 0);
        Vector3Int point2 = new Vector3Int(r2.GetCenter().x, r2.GetCenter().y, 0);


        for (int i = Mathf.Min(point1.x, point2.x) ; i <= Mathf.Max(point2.x, point1.x); ++i) //spawns horizontal and vertical tunnels at the two centres ||TODO reverse it
        {
                gameGrid[i, point2.y].walkable = true;
        }
        for (int j = Mathf.Min(point1.y, point2.y); j <= Mathf.Max(point2.y, point1.y); ++j)
        {
                gameGrid[point1.x, j].walkable = true;
        }
    }

    public void GenerateMap( GameObject player, int maxRoom, int minHeight, int maxHeight, int minWidth, int maxWidth, int EnemyCount, int maxItems)//Generates new map
    {
        for (int i = 0; i < gameGrid_x; ++i)// fills 2d array with regular tiles
            for (int j = 0; j < gameGrid_y; ++j)
            {
                gameGrid[i, j] = new Tile(new Vector3(bottom_x + i, bottom_y + j, 0), false, i, j);
            }


        for (int i = 0; i < maxRoom; ++i)
        {
            RectangleRoom newRoom = new RectangleRoom(Random.Range(mapRangeX1 , mapRangeX2), Random.Range(mapRangeY1, mapRangeY2), Random.Range(minHeight, maxHeight), Random.Range(minWidth, maxWidth)); 
            bool ok = true;
            foreach (RectangleRoom r in Rooms)
            {
                if (newRoom.GetIntersect(r))
                    ok = false;
            }

            if (ok == true)
            {
                CreateRoom(newRoom);
                Rooms.Add(newRoom);
            }

        }//creates rooms within the map with random covering random dimensions

        for ( int i = 1; i < Rooms.Count; ++i)//generates tunnels between rooms
            CreateTunnel(Rooms[i], Rooms[i-1]);

        RenderAll();
        GenerateActors(player, EnemyCount);
        GenerateItems(maxItems);
    }

    public void RenderAll() 
    {

        for (int i = 0; i < gameGrid_x; ++i) //render tiles
            for (int j = 0; j < gameGrid_y; ++j)
            {
                if (gameGrid[i, j].walkable)
                {
                    floorTiles.SetTile((floorTiles.WorldToCell(gameGrid[i, j].position)), floor_sprite);
                }
                else
                {
                    foreach(Tile t in GetNeighbouringTiles(gameGrid[i, j]))
                    {
                        if(t.walkable)
                            wallTiles.SetTile((wallTiles.WorldToCell(gameGrid[i, j].position)), wall_sprite);
                    }
                }
            }

        stairGridPos = Rooms[Rooms.Count - 1].GetCenter();//create stairs leading to next floor
        stairWorldPos = gameGrid[stairGridPos.x, stairGridPos.y].tilePostion;
        floorTiles.SetTile(floorTiles.WorldToCell( gameGrid[stairGridPos.x, stairGridPos.y].position ), nextFloor_sprite);
    }

    public void GenerateActors(GameObject player, int EnemyCount)//Generates enemies and adds player into the Entities list
    {
        player.transform.position = gameGrid[Rooms[0].GetCenter().x, Rooms[0].GetCenter().y].tilePostion;

        for (int n = 0; n < EnemyCount; ++n)//creates random enemy then stores in Entities list
        {
            GameObject enemy = Instantiate( Database._instance.GetRandomEnemy());
            Actor enemyActor = enemy.GetComponent<Actor>();
            enemyActor.fighter.parent = enemyActor;
            enemyActor.ai = new HostileEnemy(enemyActor);

            Entities.Add(enemyActor);
        }

        for(int i = 0; i < Actors.Count; ++i)
        {
            Vector3Int spawn = RandomSpawn();
            if (SpawnCheck(spawn))
            {
                Actors[i].gameObject.transform.position = gameGrid[spawn.x, spawn.y].tilePostion;
                Actors[i].Death += OnEntityDeath;
            }
            else
                --i;
        }
    }

    public void GenerateItems(int maxItems)
    {
        for(int i = 0; i < maxItems; ++i)
        {
            Vector3Int spawn = RandomSpawn();
            if (SpawnCheck(spawn))
            {
                GameObject item = Instantiate( Database._instance.GetRandomItem(), gameGrid[spawn.x, spawn.y].tilePostion, Quaternion.identity);//coin position reserved at index 0
                Entities.Add(item.GetComponent<Item>());
                item.GetComponent<Item>().Death += OnEntityDeath; //add function onentitydeath to item event
            }
        }
    }
    
    public bool SpawnCheck(Vector3Int gridPos)
    {
        return !GetEntityAt(gridPos) && gameGrid[gridPos.x, gridPos.y].walkable && gridPos != stairGridPos;
    }

    public Vector3Int RandomSpawn()
    {
        int r = Random.Range(1, Rooms.Count);
        int spawnX = Random.Range(Rooms[r].x1, Rooms[r].x2);
        int spawnY = Random.Range(Rooms[r].y1, Rooms[r].y2);
        return new Vector3Int(spawnX, spawnY, 0);
    }


    public void TileMapToGridMap()
    {
        for (int i = 0; i < gameGrid_x; ++i)// fills 2d array with regular tiles
            for (int j = 0; j < gameGrid_y; ++j)
            {
                gameGrid[i, j] = new Tile(new Vector3(bottom_x + i, bottom_y + j, 0), false, i, j);
            }

        for (int i = 0; i < gameGrid_x; ++i) //render tiles
            for (int j = 0; j < gameGrid_y; ++j)
            {
                if (floorTiles.HasTile(Vector3Int.FloorToInt(gameGrid[i, j].position)))
                {
                    gameGrid[i, j].walkable = true;
                }
            }
    }



    //Entity related helper methods
    public Entity GetEntityAt( Vector3 worldPosition) //Generic look for entity
    {
        foreach (Entity e in Entities)
        {
            if (e.transform.position == worldPosition)
            { return e; }  
        }
        return null;
        
    }

    public Actor GetActorAt( Vector3 worldPosition)
    {
        foreach( Actor e in Actors)
        {
            if (e.transform.position == worldPosition)
                return e;
        }
        Debug.Log("No ACTOR @: " + worldPosition);
        return null;
    }

    public Item GetItemAt( Vector3 worldPosition)
    {
        foreach (Entity e in Entities)
        {
            if (e.transform.position == worldPosition && e is Item)
            { return (Item)e; }
        }
        return null;
    }

    public void AddEntity(Entity entity, Vector3 worldPostion)
    {
        GameObject newObj = (GameObject)Instantiate(entity.gameObject, worldPostion, Quaternion.identity);
        Entities.Add(newObj.GetComponent<Entity>());
        newObj.GetComponent<Entity>().Death += OnEntityDeath;
    }

    //Events
    public void OnEntityDeath(Entity entity)
    {
        Entities.Remove(entity);
        entity.Death -= OnEntityDeath;
        if (entity is Actor && ((Actor)entity).ai != null)
        {
            string itemToDrop = ((Actor)entity).ai.drop_item;
            GameObject item = Database._instance.GetObjectInItems(itemToDrop);

            GameObject newObj = (GameObject)Instantiate(item, entity.transform.position, Quaternion.identity);
            Entities.Add(newObj.GetComponent<Entity>());
            newObj.GetComponent<Item>().Death += OnEntityDeath;
        }
        entity.selfDistruct();
    }




    //Grid Related Methods
    public List<Tile> GetNeighbouringTiles(Tile t)
    {
        List<Tile> neighbours = new List<Tile>();

        if (t.gridX >= 0 && t.gridX <= gameGrid_x - 1 && t.gridY >= 0 && t.gridY < gameGrid_y - 1) //check top
            neighbours.Add(gameGrid[t.gridX, t.gridY + 1]);

        if (t.gridX >= 0 && t.gridX < gameGrid_x - 1 && t.gridY >= 0 && t.gridY < gameGrid_y - 1)//check right
            neighbours.Add(gameGrid[t.gridX + 1, t.gridY]);

        if (t.gridX > 0 && t.gridX < gameGrid_x - 1 && t.gridY >= 0 && t.gridY < gameGrid_y - 1)//check left
            neighbours.Add(gameGrid[t.gridX - 1, t.gridY]);

        if (t.gridX >= 0 && t.gridX < gameGrid_x - 1 && t.gridY > 0 && t.gridY < gameGrid_y - 1)//check bottom
            neighbours.Add(gameGrid[t.gridX, t.gridY - 1]);


        return neighbours;
    }
    public Tile WorldToTile(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x - 0.5f + (gameGrid_x / 2)); //only accounts for when grid's position is  (0,0,0) (-0.5 for offset because unity likes to round up for some reason)
        int y = Mathf.RoundToInt(worldPosition.y - 0.5f + (gameGrid_y / 2));
        return gameGrid[x, y];
    }




    //Editor related methods
   private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gameGrid_x, gameGrid_y, 1)); //debug tool
        foreach (Tile t in gameGrid)
        {
            if (t != null)
            { //write entity and item checks 
                Gizmos.color = t.walkable ? Color.white : Color.red;
                foreach (Entity e in Entities)
                {
                    if (WorldToTile(e.transform.position) == t)
                        Gizmos.color = Color.green;
                }
                Gizmos.DrawCube(t.tilePostion, Vector3.one * (0.5f));
            }
        }
    } 
}
