using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAct;


public class ElectricDamageConsumable : Consumable
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
        foreach (Actor actor in action.engine.Grid.Actors)
        {
            if (actor.transform.position == action.location)
            {
                actor.fighter.hp -= damage;
                break;
            }
        }
        action.engine.effectsManager.InstantiateEffect(Effect, action.location);
        if (user is Actor)
            ((Actor)user).inventory.RemoveItem(action.item);
    }

}