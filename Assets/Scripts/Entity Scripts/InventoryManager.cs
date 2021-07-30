using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAct;

/*
    public override void GetMove(GameManager engine, GameObject player)
    {
        Debug.Log("In inventory");


        if (Input.anyKeyDown)
        {
            string key = Input.inputString.ToString();
            Action action = null;
            switch (key)
            {
                case "1":
                    action = OnItemUse(0, playerActor);
                    break;
                case "2":
                    action = OnItemUse(1, playerActor);
                    break;
                case "3":
                    action = OnItemUse(2, playerActor);
                    break;
                case "4":
                    action = OnItemUse(3, playerActor);
                    break;
                default:
                    Debug.Log("Invalid Input: " + key);
                    break;
            }
            if(action != null)
            {
                action.Perform();
                engine.uiManager.CreateEntityInventory(playerActor);
            }

        }
    }

    public Action OnItemUse(int slot, Actor actor)
    {
        return new ItemAction(actor, actor.inventory.items[slot]);
    }
    */


