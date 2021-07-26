using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MovingEntity
{
    public MovingEntity player; //create target on instantiate

    public override EntityAction GetAction()
    {
        if (entityPosition.neighbourNodes.Contains(player.entityPosition))
        {
            Debug.Log("attack!"); //todo attack action
        }
        else
        {
            return EntityAction.move;
        }

        return EntityAction.wait;

        //base.GetAction(); //maybe use this for energy allocation?
    }

}
