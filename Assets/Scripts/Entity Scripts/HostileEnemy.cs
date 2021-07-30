using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAct;
public class HostileEnemy : BaseAi
{
    List<Tile> path = new List<Tile>();

    public int attackRange;
    public GameObject hitEffect;
    
     void Start()
    {
        parent = GetComponent<Actor>();
    }

    public virtual void Perform(GameManager engine, Entity target)
    {
        float dx = (target.transform.position.x - parent.transform.position.x); //distance to player of x
        float dy = (target.transform.position.y - parent.transform.position.y); // distance to player of y
        float distance = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));


        if (distance <= attackRange && !(Mathf.Abs(dx) > 0 && Mathf.Abs(dy) > 0) && LineOfSight(target))//second case prevents attacks from diagonal
        {
            new MeleeAction(hitEffect, this.parent, (int)dx, (int)dy).Perform(engine);
        }
        else if (Mathf.Abs((parent.transform.position - target.transform.position).magnitude) < engine.lightingRenderer.playerLightRadius)
        {
            get_path_to(engine, parent, target);//pathfind and then move
        }

        new WaitAction().Perform(engine);

    }


    public override int TakeTurn(GameManager engine)
    {
        Perform(engine, engine.player.GetComponent<Actor>());
        return Fighter.actionCost;
    }

}
