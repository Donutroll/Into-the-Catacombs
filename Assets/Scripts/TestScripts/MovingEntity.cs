using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingEntity : MonoBehaviour
{
   public enum EntityAction
    {
       move = 0,
       attack = 1,
       wait = 2
    }

    public GameObject Entity;
    public int health = 1;
    public int attack = 1;
    public int energy = 0;
    public int energyPerTurn = 5;
    public Tile entityPosition;
   
    public virtual EntityAction GetAction()
    {
        return EntityAction.wait;
    }



    public virtual void EntityDeath()
    {

    }

    public virtual void EntityAttack()
    {

    }

    public virtual void EntityMove()
    {

    }

}
