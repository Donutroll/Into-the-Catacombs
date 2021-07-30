using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAct;

public class FireDamageConsumable : Consumable
{
    public int damage = 0;
    public int radius = 0;

    protected override void Start()
    {
        base.Start();
    }

    public override Action GetAction(InventoryEvent ev)
    {
        ev.engine.eventManager = new TargetingAreaEvent(ev.engine, radius, (Vector3 xy) => { new ItemAction(ev.player.GetComponent<Actor>(), ev.itemToUse, xy).Perform(ev.engine); });
        return null;
    }
    public override void Activate(ItemAction action)
    {
        Entity user = action.entity;
        GridMap gridMap = action.engine.Grid;

        Tile castTile = gridMap.WorldToTile(action.location);
        Rooms.RectangleRoom areaToHit = new Rooms.SquareRoom(castTile.gridX - radius, castTile.gridY - radius, radius * 2);

        for (int i = 0; i < gridMap.Actors.Count; ++i)
        {
            Vector3 actorPos = gridMap.Actors[i].transform.position;
            float distance = Mathf.Sqrt(Mathf.Pow((action.location.x - actorPos.x), 2) + Mathf.Pow((action.location.y - actorPos.y), 2));
            if (distance <= radius)
                gridMap.Actors[i].fighter.hp -= damage;
        }

        for (int x = areaToHit.x1; x < areaToHit.x2; ++x)//create fire sprites
            for (int y = areaToHit.y1; y < areaToHit.y2; ++y)
            {
                if (gridMap.gameGrid[x, y].walkable && Mathf.Abs((gridMap.gameGrid[x, y].position - castTile.position).magnitude) < radius)
                    action.engine.effectsManager.InstantiateEffect(Effect, gridMap.gameGrid[x, y].tilePostion);
            }

        if (user is Actor)
            ((Actor)user).inventory.RemoveItem(action.item);

    }
}