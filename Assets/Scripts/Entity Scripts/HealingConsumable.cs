using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAct;

public class HealingConsumable : Consumable
{
    public int amount = 0;

    protected override void Start()
    {
        base.Start();
    }

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