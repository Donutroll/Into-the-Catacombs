using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameAct;

//override with this.onselected with askfor events
public class EventManager
{
    public GameManager engine;
    public GameObject player;
    public EventManager(GameManager engine, GameObject player)
    {
        this.engine = engine;
        this.player = player;
    }

    public EventManager(GameManager engine)
    {
        this.engine = engine;
        this.player = engine.player;
    }

    public void HandleAction(GameAct.Action action)
    {
        if (action == null)
        {
            return;
        }
        
        action.Perform(engine);
        HandleEnemyTurn();
    }

    public virtual void HandleEvent()
    {
        Debug.LogError("EventManager HandleEvent was called, somethings gone wrong");
    }


    public void HandleEnemyTurn()
    {
        GridMap Grid = engine.Grid;
        if (Grid.Actors.Count > 1)
        {
            for (int i = 1; i < Grid.Actors.Count; ++i)
            {
                Actor actor = Grid.Actors[i];
                if (GameManager.VectorDistance(actor.gameObject, player) < engine.lightingRenderer.playerLightRadius - 0.5f)
                {
                    actor.fighter.Energy += actor.fighter.recoverySpeed;
                    while (actor.fighter.Energy >= Fighter.actionCost)
                    {
                        actor.fighter.Energy -= actor.ai.TakeTurn(engine);
                    }
                }
            }
        }

    }

    public List<KeyCode> ExitLog = new List<KeyCode>() { KeyCode.Escape, KeyCode.Q };

    public List<KeyCode> Enterlog = new List<KeyCode>() { KeyCode.E, KeyCode.Return, KeyCode.Space };


    public Tuple<int, int> MoveLog(KeyCode input)
    {
        switch (input)
        {
            case KeyCode.W:
                return new Tuple<int, int>(0, 1);
            case KeyCode.A:
                return new Tuple<int, int>(-1, 0);
            case KeyCode.S:
                return new Tuple<int, int>(0, -1);
            case KeyCode.D:
                return new Tuple<int, int>(1, 0);
            default:
                break;
        }
        return null;
    }

}

public class AskForEvent : EventManager
{
    public AskForEvent(GameManager engine, GameObject player) : base(engine, player)
    { }

    public AskForEvent(GameManager engine) : base(engine)
    { }

    public void OnExit()
    {
        Debug.Log("Quitting a subclass of AskForEvent");
        engine.eventManager = new MainGameEvent(this.engine);
    }
}




public class GameOverEvent : EventManager
{
    public GameOverEvent(GameManager engine) : base (engine)
    { }
    public override void HandleEvent()
    {
    }

    public void OnExit()
    {
        engine.eventManager = new MainGameEvent(this.engine);
    }
}

public class PauseScreenEvent : EventManager
{
    public PauseScreenEvent(GameManager engine) : base (engine)
    {
        engine.uiManager.CreatePause();
    }

    public override void HandleEvent()
    {
       foreach( KeyCode k in engine.inputManager.inputList)
        {
            if (ExitLog.Contains(k))
                engine.uiManager.ExitPause();
        }
        engine.inputManager.inputList.Clear();
    }

    public void OnExit()
    {
        engine.eventManager = new MainGameEvent(this.engine);
    }

}

public class MainGameEvent : EventManager
{
    public MainGameEvent(GameManager engine, GameObject player) : base(engine, player)
    { }

    public MainGameEvent(GameManager engine) : base(engine)
    {}
    

    public override void HandleEvent()
    {
        GameAct.Action action = null;
        foreach (KeyCode k in engine.inputManager.inputList)
        {
            if (MoveLog(k) != null)
            {
                int dx = MoveLog(k).Item1;
                int dy = MoveLog(k).Item2;
                action = new ChoiceAction(player.GetComponent<Actor>(), dx, dy);
            }
            else if( k == KeyCode.I)
            {
                engine.eventManager = new UseInventoryItem(engine);
            }
            else if(k == KeyCode.Escape)
            {
                engine.eventManager = new PauseScreenEvent(engine);
            }
            else if( k == KeyCode.O)
            {
                engine.eventManager = new DropInventoryItem(engine);
            }
            else if( k == KeyCode.Space)
            {
                action = new NextFloorAction();
            }
            base.HandleAction(action);
        }
        engine.inputManager.inputList.Clear();

    }


}





public class InventoryEvent :  AskForEvent
{
    public UiManager ui;
    public Inventory playerInventory;
    public Item itemToUse;

    int _inventorySlot;
    public int inventorySlot
    {
        get => _inventorySlot;
        set
        {
          if (value > playerInventory.capacity - 1)
                _inventorySlot = playerInventory.capacity - 1;
            else if (value < 0)
                _inventorySlot = 0;
            else
                _inventorySlot = value;
        }
    }

    public InventoryEvent(GameManager engine) : base(engine)
    {
        ui = engine.uiManager;
        ui.SelectionBorder.SetActive(true);
        ui.SelectionBorder.transform.position = ui.InventorySlots[0].transform.position;

        _inventorySlot = 0;
        playerInventory = engine.player.GetComponent<Actor>().inventory;
        itemToUse = null;
    }

    public override void HandleEvent()
    {
        foreach (KeyCode k in engine.inputManager.inputList)
        {
            switch (k)
            {
                case KeyCode.Return:
                case KeyCode.E:
                    if (inventorySlot >= playerInventory.items.Count)
                        break;
                    itemToUse = playerInventory.GetItem(inventorySlot);
                    OnSelected();
                    return;
                case KeyCode.Q:
                case KeyCode.I:
                case KeyCode.Escape:
                    ui.SelectionBorder.SetActive(false);
                    OnExit();
                    break;
                case KeyCode.D:
                    ++inventorySlot; //get , modify then set.
                    break;
                case KeyCode.A:
                    --inventorySlot;
                    break;
                case KeyCode.W:
                    inventorySlot -= 2;
                    break;
                case KeyCode.S:
                    inventorySlot += 2;
                    break;
                default:
                    break;

            }
            ui.SelectionBorder.transform.position = ui.InventorySlots[inventorySlot].transform.position;
        }
        engine.inputManager.inputList.Clear();
    }

    public virtual void OnSelected()
    {
        Debug.Log("InventoryEvent on selected was called, something has gone wrong");
    }

}

public class UseInventoryItem : InventoryEvent
{
    public UseInventoryItem(GameManager engine) : base(engine)
    {
        
    }

    public override void HandleEvent() => base.HandleEvent();

    public override void OnSelected()
    {
        OnExit();
        ui.SelectionBorder.SetActive(false);
        base.HandleAction(itemToUse.consumable.GetAction(this)); //calls an intermediate from the consumable component
    }
}

public class DropInventoryItem : InventoryEvent
{
    public DropInventoryItem(GameManager engine) : base(engine)
    {

    }

    public override void HandleEvent() => base.HandleEvent();


    public override void OnSelected()
    {
        OnExit();
        ui.SelectionBorder.SetActive(false);
        base.HandleAction(new DropItemAction(player.GetComponent<Actor>(), itemToUse));
    }
}



public class TargetingEvent : AskForEvent
{
    public GameObject selectionBorder;
    Vector3 _targetLocation;

    public Vector3 targetLocation
    {
        get => _targetLocation;
        set
        {
            _targetLocation = value;
            selectionBorder.transform.position = _targetLocation;
        }
    }

    public TargetingEvent(GameManager engine) : base(engine)
    {
        selectionBorder = engine.CreateObject(engine.inputManager.SelectionBorder); 
        targetLocation = player.transform.position;
    }


    public override void HandleEvent()
    {
        foreach (KeyCode k in engine.inputManager.inputList)
        {
            if(MoveLog(k) != null)
            {
                int dx = MoveLog(k).Item1;
                int dy = MoveLog(k).Item2;
                targetLocation += new Vector3(dx, dy);
            }
            else if(ExitLog.Contains(k))
            {
                selectionBorder.SetActive(false);
                OnExit();
            }
            else if(Enterlog.Contains(k) && targetLocation != player.transform.position && engine.Grid.WorldToTile(targetLocation).walkable)
            {
                Debug.Log("selected");
                this.OnSelected(); //calls subclass
            }
        }
        engine.inputManager.inputList.Clear();
    }

    public virtual void OnSelected()
    {
        Debug.Log("targetingEvent on selected was called, something has gone wrong");
    }

}

public class TargetingSingleEvent : TargetingEvent
{
    public TargetingSingleEvent(GameManager engine) : base(engine)
    {
    }



    public override void HandleEvent()
    {
        base.HandleEvent();
    }

    public override void OnSelected()
    {

    }

}

public class TargetingAreaEvent : TargetingEvent
{
    int radius;
    System.Action<Vector3> returnAction;
    public TargetingAreaEvent(GameManager engine, int radius, System.Action<Vector3> toCall ) : base(engine)
    {
        selectionBorder.transform.localScale = new Vector3(0.5f,0.5f);
        this.returnAction = toCall;
    }

    public override void HandleEvent()
    {
        base.HandleEvent();
    }

    public override void OnSelected()
    {
        OnExit();
        engine.DestroyObject(selectionBorder);
        returnAction(targetLocation);
    }
}