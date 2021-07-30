using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAct;

public class Consumable : MonoBehaviour
{
    protected virtual void Start()
    {
        parent = GetComponent<Item>();
    }

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
