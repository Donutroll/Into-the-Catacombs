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



    public int chestSpawnChance;
    public int PotRoomChance;
    public int PotRoomMin;
    public int MinEnemyPerRoom;
    public int MinItemPerRoom;
    public int MaxEnemyPerRoom;
    public int MaxItemPerRoom;
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
                    if(tmpE.ai != null)
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
    ///    REMINDER: Each tile on the world has it's coordinates set on the bottom left corner. Generation of a map
    ///    propogrates from the bottom left corner of the world. 
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


        for (int i = Mathf.Min(point1.x, point2.x) ; i <= Mathf.Max(point2.x, point1.x); ++i) //spawns horizontal and vertical tunnels at the two centres
        {
                gameGrid[i, point2.y].walkable = true;
        }
        for (int j = Mathf.Min(point1.y, point2.y); j <= Mathf.Max(point2.y, point1.y); ++j)
        {
                gameGrid[point1.x, j].walkable = true;
        }
    }

    public void GenerateMap( GameObject player, int maxRoom, int minHeight, int maxHeight, int minWidth, int maxWidth, int maxEnemies, int maxItems)//Generates new map
    {
        for (int i = 0; i < gameGrid_x; ++i)// fills 2d array with regular tiles
            for (int j = 0; j < gameGrid_y; ++j)
            {
                gameGrid[i, j] = new Tile(new Vector3(bottom_x + i, bottom_y + j, 0), false, i, j);
            }


        for (int i = 0; i < maxRoom; ++i)//creates random amount of rooms
        {
            RectangleRoom newRoom = new RectangleRoom( this, Random.Range(mapRangeX1 , mapRangeX2), Random.Range(mapRangeY1, mapRangeY2), Random.Range(minHeight, maxHeight), Random.Range(minWidth, maxWidth)); 
            bool ok = true;
            foreach (RectangleRoom r in Rooms)
            {
                if (newRoom.GetIntersect(r))
                    ok = false;
            }

            if (ok == true)
            {
                CreateRoom(newRoom);//move laters
                Rooms.Add(newRoom);
            }

        }//creates rooms within the map with random covering random dimensions


        for (int i = 1; i < Rooms.Count; ++i)
            CreateTunnel(Rooms[i], Rooms[i - 1]);


        for (int i = 1; i < Rooms.Count - 1; ++i)//generates tunnels between rooms and populate rooms
        {
            int enemyCount = Random.Range(MinEnemyPerRoom, MaxEnemyPerRoom);
            int itemCount = Random.Range(MinItemPerRoom, MaxItemPerRoom);
            maxEnemies -= enemyCount;
            maxItems -= itemCount;

            if (maxItems >= 0 || maxEnemies >= 0)
                PopulateRoom(Rooms[i], enemyCount, itemCount);
            else
                continue; 
            
        }

        RenderAll();

        player.transform.position = gameGrid[Rooms[0].GetCenter().x, Rooms[0].GetCenter().y].tilePostion;

    }

    public void PopulateRoom(RectangleRoom room, int enemyCount, int itemCount)
    {

        List<Tile> edgeTiles = new List<Tile>(); //seperates tiles on the edge of the room and inner tiles
        List<Tile> RemainderRoomTiles = new List<Tile>();

        for (int i = 0; i < room.RoomTiles.Count; ++i)
        {
            Tile t = room.RoomTiles[i];
            RemainderRoomTiles.Add(t);

            if (t.gridX == room.x1 || t.gridX == room.x2 || t.gridY == room.y1 || t.gridY == room.y2 && (t.gridX != room.GetCenter().x || t.gridY != room.GetCenter().y))
            {
                edgeTiles.Add(t);
                RemainderRoomTiles.Remove(t);
            }
        }

        int r1 = Random.Range(1, 20);

        if (r1 > chestSpawnChance) //spawn chest
        {
            Debug.Log("spawning chest");
            GameObject chest = Database._instance.GetObjectFromInteractable("Chest");
            AddEntity(chest.GetComponent<Entity>(), gameGrid[room.GetCenter().x, room.GetCenter().y].tilePostion);
            RemainderRoomTiles.Remove(gameGrid[room.GetCenter().x, room.GetCenter().y]);
        }


        int r2 = Random.Range(1, 20);

        if (r2 > PotRoomChance) //spawn pots
        {
            Debug.Log("spawning pot");
            for (int i = 0; i < Random.Range(PotRoomMin, edgeTiles.Count); ++i)
            {
                int index = Random.Range(0, edgeTiles.Count - 1);
                Tile spawn = edgeTiles[index];
                if (SpawnCheck(new Vector3Int(spawn.gridX, spawn.gridY, 0)))
                {
                    GameObject Pot = Database._instance.GetObjectFromInteractable("Pot");
                    AddEntity(Pot.GetComponent<Entity>(), spawn.tilePostion);
                    edgeTiles.RemoveAt(index);
                }
            }
        }

        if (enemyCount + itemCount > room.RoomTiles.Count)
        {
            enemyCount = MinEnemyPerRoom;
            itemCount = MinItemPerRoom;
        }


        for (int i = 0; i < enemyCount; ++i) //spawn enemies
        {
            int index = Random.Range(0, RemainderRoomTiles.Count - 1);
            Tile spawn = RemainderRoomTiles[index];
            if (SpawnCheck(new Vector3Int(spawn.gridX, spawn.gridY, 0)))
            {
                GameObject enemy = Database._instance.GetRandomEnemy();
                AddEntity(enemy.GetComponent<Entity>(), spawn.tilePostion);
                RemainderRoomTiles.RemoveAt(index);
            }
            else --i;

        }

        for (int i = 0; i < itemCount; ++i) //spawn items
        {
            int index = Random.Range(0, RemainderRoomTiles.Count - 1);
            Tile spawn = RemainderRoomTiles[index];
            if (SpawnCheck(new Vector3Int(spawn.gridX, spawn.gridY, 0)))
            {
                GameObject enemy = Database._instance.GetRandomItem();
                AddEntity(enemy.GetComponent<Entity>(), spawn.tilePostion);
                RemainderRoomTiles.RemoveAt(index);
            }
            else --i;

        }
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



    public bool SpawnCheck(Vector3Int gridPos)
    {
        foreach(Entity e in Entities)
        {
            if (e.transform.position == gridPos + new Vector3(0.5f, 0.5f, 0))//+0.5f to the center of the tile
                return false;
        }
        return gameGrid[gridPos.x, gridPos.y].walkable && gridPos != stairGridPos; 
    }


    public Vector3Int RandomGridSpawn()
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

            AddEntity(item.GetComponent<Item>(), entity.transform.position);
        }
        else if(entity is Interactable)
        {
            GameObject toInsantiate = ((Interactable)entity).OnTrigger();
            AddEntity(toInsantiate.GetComponent<Item>(), entity.transform.position);
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
        foreach (Tile t in gameGrid)// differentiates between entities, walkable and unwalkable spaces
        {
            if (t != null)
            { 
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
