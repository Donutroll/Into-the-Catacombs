using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game;



//Actor related components

[System.Serializable]
public class Fighter 
{
    public Actor parent = null;

    public int maxHp, currHp, defense, power;

    public Fighter(int hp, int defense, int power, Actor parent)
    {
        this.maxHp = hp;
        this.currHp = hp;
        this.defense = defense;
        this.power = power;
        this.parent = parent;
    }


    public int Heal(int amount)
    {
        if (hp == maxHp)
            return 0;
        hp += amount;

        return amount;
    }

    public int hp
    {
        set
        {
            int damageTaken = value - currHp;
            currHp = Mathf.Max(0, Mathf.Min(value, maxHp)); //value is the modified hp
            parent.ActorDamaged(this, damageTaken);
            if (hp <= 0)
                Die();
        }

        get
        {
            return currHp;
        }
    }

    public void Die()
    {
        if (parent.type == "Player")
        {
            Debug.Log(" You died! ");
        }
        Debug.Log(parent.type + "'s lifeless corpse topples to the ground");
        parent.EntityDeath(parent);
        parent.ai = null;
        parent.blockMove = false;
        parent.type = "dead";
        parent.gameObject.GetComponentInChildren<SpriteRenderer>().sprite = null;

    }
}

public class BaseAI : Action
{
    public BaseAI()
    {

    }

    public Actor parent = null;

    public string drop_item = "coin";

    public List<Tile> get_path_to(GameManager engine, Entity self , Entity target) //creates a list of tiles towards player
    {
        Tile[,] pathMap = engine.Grid.gameGrid;

        foreach ( Entity e in engine.Grid.Entities)
        {
            if (e.blockMove || e is Item)//prevents enemies from gathering by making surroundings more difficult to traverse
            {
                Tile entityTile = engine.Grid.WorldToCell(e.transform.position);
                foreach ( Tile t in engine.Grid.GetNeighbouringTiles(entityTile))
                    pathMap[t.gridX,t.gridY].mCost += 30;
            }
        }

        Tile selfTile = engine.Grid.WorldToCell(self.transform.position);
        Tile targetTile = engine.Grid.WorldToCell(target.transform.position);

        List<Tile> returnPath = new PathFinding(pathMap, selfTile, targetTile).GetPath();

        return returnPath;
           
    }
}

public class HostileEnemy : BaseAI
{

    List<Tile> path = new List<Tile>();

    public HostileEnemy(Actor acting)
    {
        parent = acting;
    }


    public override void Perform(GameManager engine, Entity target) 
    {
        int dx = (int)(target.transform.position.x - parent.transform.position.x); //distance to player of x
        int dy = (int)(target.transform.position.y - parent.transform.position.y); // distance to player of y
        int distance = Mathf.Max( Mathf.Abs(dx), Mathf.Abs(dy)); // will get the axis coordinates that is closest. You will hate me for this

        if (distance <= 1 && !(Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1))
            new MeleeAction(dx, dy).Perform(engine, parent); //passes destination of attack by dx, dy
        else if( distance <= 5) 
        {
            path = get_path_to(engine, parent, target);

            if(path != null)
            {
                Tile toTile = path[0];
                path.RemoveAt(0);
                new MovementAction((int)(toTile.tilePostion.x - parent.transform.position.x), (int)(toTile.tilePostion.y - parent.transform.position.y)).Perform(engine, parent);
            }
        }

        new WaitAction().Perform();

    }
}



[CreateAssetMenu(fileName = "Consumable", menuName = "Items / Consumable")]
public class Consumable : ScriptableObject
{
    public Item parent = null;
    public GameObject Effect;
    public virtual Action GetAction(InventoryEvent ev)
    {
       return new ItemAction(ev.player.GetComponent<Actor>(), ev.itemToUse);
    }

    public virtual void Activate(ItemAction action)
    {
        Debug.Log("huh? this shouldn't be called");
    }

}


[CreateAssetMenu(fileName = "HealingConsumable", menuName = "Items / HealingConsumable")]
public class HealingConsumable : Consumable
{
    public int amount = 0;

    public override void Activate(ItemAction action)
    {
        Debug.Log("activated healing");
        Entity user = action.entity;
        int amountHealed = user.fighter.Heal(this.amount);

        if (amountHealed > 0)
        {
            Debug.Log(user.type + " recovered " + amountHealed + " by consuming " + this.parent.type);
            if (user is Actor)
                ((Actor)user).inventory.RemoveItem(action.item);
        }
        else
        {
            Debug.Log("You're health is full");
        }

    }

}

[CreateAssetMenu(fileName = "FireDamageConsumable", menuName = "Items / FireDamageConsumable")]
public class FireDamageConsumable : Consumable
{
    public int damage = 0;
    public int radius = 0;
    
    public override Action GetAction(InventoryEvent ev)
    {
        ev.engine.eventManager = new TargetingAreaEvent(ev.engine, radius, (Vector3 xy) => { new ItemAction(ev.player.GetComponent<Actor>(), ev.itemToUse, xy).Perform(ev.engine, ev.player.GetComponent<Actor>()); } );
        return null;
    }
    public override void Activate(ItemAction action)
    {
        Entity user = action.entity;
        GridMap gridMap = action.engine.Grid;

        Tile castTile = gridMap.WorldToCell(action.location);
        Rooms.RectangleRoom areaToHit = new Rooms.RectangleRoom((int)castTile.gridX - radius, (int)castTile.gridY - radius, radius * 2, radius * 2);

        for( int i = 0; i < gridMap.Actors.Count; ++i)
        {
            Vector3 actorPos = gridMap.Actors[i].transform.position;
            float distance = Mathf.Sqrt(Mathf.Pow((action.location.x - actorPos.x), 2) + Mathf.Pow((action.location.y - actorPos.y), 2));
            if (distance < radius)
                gridMap.Actors[i].fighter.hp -= damage;
        }

        for (int x = areaToHit.x1; x < areaToHit.x2; ++x)//create fire sprites
            for (int y = areaToHit.y1; y < areaToHit.y2; ++y)
            {
                if (gridMap.gameGrid[x, y].walkable)
                    action.engine.effectsManager.InstantiateEffect(Effect, gridMap.gameGrid[x, y].tilePostion);
            }

        if (user is Actor)
            ((Actor)user).inventory.RemoveItem(action.item);

    }
}

public class ElectricDamageConsumable
{

}




[System.Serializable]
public class Inventory 
{
    public delegate void OnInventory();
    public event OnInventory OnInventoryChange; 
    
    public Actor parent = null;
    public int capacity;

    public List<Item> items = new List<Item>();
    public Inventory(int capacity)
    {

        this.capacity = capacity;
    }


    public void AddItem(Item item)
    {
        items.Add(item);
        OnInventoryChange?.Invoke();
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
        Debug.Log("removed " + item.type);
        OnInventoryChange?.Invoke();
    }


    public Item GetItem(int slot)
    {
        if (slot >= items.Count)
        {
          Debug.Log("invalid slot: " + slot);
          return null; 
        }
        else
            return items[slot];
    }
}

