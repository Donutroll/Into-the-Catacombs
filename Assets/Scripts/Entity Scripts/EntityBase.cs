using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAct;



//Actor related components

[System.Serializable]
public class Fighter 
{
    public Actor parent = null;

    public int maxHp, currHp, defense, power, recoverySpeed;
    public const int actionCost = 100;
    private int _Energy;
    public int Energy
    {
        get
        {
            return _Energy;
        }
        set
        {
            if (value < 0)
                _Energy = 0;
            else
                _Energy = value;
        }
    }

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

