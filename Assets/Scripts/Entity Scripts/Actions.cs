using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GameAct
{
    public class Action 
    {
        public GameManager engine;

       //abstract class for virtual inheritance


        public virtual void Perform(GameManager engine)
        {
            Debug.Log("Action Perform called, somethings gone wrong?");
        }

        public virtual void Perform()
        {
            Debug.Log("Action Perform called, somethings gone wrong?");
        }
    }

    public class WaitAction : Action
    {
        public override void Perform(GameManager engine)
        {
        }
    }

    public class ItemAction : Action
    {
        public Entity entity;
        public Item item;
        public Vector3 location;

        public ItemAction()
        { }
        public ItemAction(Entity entity, Item item)//casting entity and item
        {
            this.entity = entity;
            this.item = item;
        }
        public ItemAction(Entity entity, Item item, Vector3 location)// + target location
        {
            this.entity = entity;
            this.item = item;
            this.location = location;
        }

        public Entity TargetEntity(GameManager engine)
        {
            return engine.Grid.GetEntityAt(location);
        }

        public override void Perform(GameManager engine)
        {
            this.engine = engine;
            item.consumable.Activate(this);
        }

    }

    public class DropItemAction : ItemAction
    {
        public Actor actor;
        public DropItemAction(Actor actor, Item item) : base(actor,item)
        {
            this.actor = actor;
        }

        public override void Perform(GameManager engine)
        {
            actor.inventory.RemoveItem(item);
            engine.Grid.AddEntity(item, actor.gameObject.transform.position);
        }
    }

    public class PickupItem : Action
    {
        public Item item;
        public Actor actor;
        public PickupItem( Item item, Actor actor)
        {
            this.item = item;
            this.actor = actor;
        }

        public override void Perform(GameManager engine)
        {
            if (item.type == "coin")
            {
                engine.uiManager.UpdateCoinCount(item.value);
                item.EntityDeath(item);
                return;
            }

            foreach (Entity e in engine.Grid.Entities)
            {
                if (e is Item && (e.transform.position == item.transform.position))
                {
                  if (actor.inventory.items.Count >= actor.inventory.capacity)
                    {
                        Debug.Log("You're inventory is full!");
                        return;
                    }
                    Debug.Log("Picked up " + item.type);
                    Item itemToGet = Database._instance.GetObjectInItems(item.type).GetComponent<Item>();
                    actor.inventory.AddItem(itemToGet);
                    item.EntityDeath(item);
                    break;
                }
            }
                
        }
    }

    public class EscapeAction : Action
    {
        public EscapeAction(GameManager engine)
        {
        }
    }

    public class DirectionAction : Action// superclass related to actions that require positions
    {
        public int dx, dy;
        public Entity entity;
        public DirectionAction(int dx , int dy )
        {
            this.dx = dx;
            this.dy = dy;
        }

        public DirectionAction(int dx, int dy, Entity entity)
        {
            this.dx = dx;
            this.dy = dy;
            this.entity = entity;
        }
    }

    public class MovementAction : DirectionAction// subclass to move
    {
        public MovementAction(Entity entity, int dx = 1, int dy = 1) : base(dx, dy, entity)
        {
        }


        public override void Perform(GameManager engine)
        {
            Vector3 destination = entity.transform.position + new Vector3(dx, dy, 0);
            Entity blockade = engine.Grid.GetEntityAt(destination);
            //Debug.Log(entity.type + " has taken a step to" + destination);
            if (!engine.Grid.WorldToTile(destination).walkable)
                Debug.Log("unwalkable space");
            else if (blockade && blockade.blockMove)
                Debug.Log("Blocking");
            else
            {
                entity.transform.position = destination;
            }

        }
    }
       
    public class MeleeAction : DirectionAction// subclass to attack
    {
        GameObject hitEffect = null;

        public MeleeAction( Entity entity, int dx = 1, int dy = 1) : base(dx, dy, entity)
        { }

        public MeleeAction(GameObject hitEffect , Entity entity, int dx = 1, int dy = 1) : base(dx, dy, entity)
        {
            this.hitEffect = hitEffect;
        }

        public override void Perform(GameManager engine)
        {
            Vector3 destination = entity.transform.position + new Vector3(dx, dy, 0);
            Entity target = engine.Grid.GetEntityAt(destination);
            if (target == null)
                Debug.LogError("No entity??? at " + destination);
            else
            {
                Debug.Log("Attack commences on " + target.type);
                int damage = entity.fighter.power - target.fighter.defense;
                if (damage > 0)
                {
                    target.fighter.hp -= damage;
                    if(hitEffect != null)
                    {
                        engine.effectsManager.InstantiateEffect(hitEffect, entity.transform.position, target.transform.position);
                    }
                }
            }

        }
    }

    public class RangedAction : DirectionAction
    {
        public RangedAction(Entity entity, int dx = 1, int dy = 1) : base(dx, dy, entity)
        { }

        public override void Perform()
        {
            
        }
    }

    public class ChoiceAction : DirectionAction //decides between other subclasses
    {
        public ChoiceAction(Entity entity, int dx = 1, int dy = 1) : base(dx, dy, entity)
        { }
        
        public override void Perform(GameManager engine)
        {

            Vector3 destination = entity.transform.position + new Vector3(dx, dy, 0);
            Entity blocker = engine.Grid.GetEntityAt(destination);
            if (blocker != null )
            {
                if( blocker is Actor)
                    new MeleeAction(((Actor)entity).HitEffect, entity, this.dx, this.dy).Perform(engine);
                else if (blocker is Item && entity.type == "Player")
                {
                    new MovementAction(entity, this.dx, this.dy).Perform(engine);
                    new PickupItem((Item)blocker, (Actor)entity).Perform(engine);
                }
            }
            else 
                new MovementAction(entity, this.dx, this.dy).Perform(engine);

        }
    }

    public class NextFloorAction : Action
    {
        public override void Perform(GameManager engine)
        {
            GridMap grid = engine.Grid;
            if (engine.player.transform.position == grid.stairWorldPos)
            {
                grid.floorTiles.ClearAllTiles();
                grid.wallTiles.ClearAllTiles();
                engine.lightingRenderer.DrawFog();
                grid.Rooms.Clear();
                for (int i = grid.Entities.Count - 1; i >= 1; --i) //iterate backwards because List.removeAt shifts index down
                {
                    if (grid.Entities[i].type != "Player")
                    {
                        grid.Entities[i].selfDistruct();
                        grid.Entities.RemoveAt(i);
                    }
                }
                engine.Grid.GenerateMap(engine.player.gameObject, 30, 4, 7, 5, 9, 15, 10);
                engine.uiManager.UpdateFloorCount();
                engine.lightingRenderer.RenderLight(engine.lightingRenderer.playerLightRadius, engine.player.GetComponent<Actor>());
            }
        }

    }
}