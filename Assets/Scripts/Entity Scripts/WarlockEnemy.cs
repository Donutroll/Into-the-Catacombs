using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameAct;

public class WarlockEnemy : HostileEnemy
{
    public List<Sprite> Sprites = new List<Sprite>(); //0 is casting, 1 is idle
    public SpriteRenderer currSprite;
    public bool casting = false;
    public string spellEffect = "Fire";
    Tile toCast;
    protected virtual void Start()
    {
        parent = GetComponent<Actor>();
        currSprite = GetComponentInChildren<SpriteRenderer>();
    }

    public override int TakeTurn(GameManager engine)
    {
        Perform(engine, engine.player.GetComponent<Actor>());
        return Fighter.actionCost;
    }

    public override void Perform(GameManager engine, Entity target)
    {
        int dx = (int)(target.transform.position.x - parent.transform.position.x); //distance to player of x
        int dy = (int)(target.transform.position.y - parent.transform.position.y); // distance to player of y
        int distance = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));




        if (distance <= attackRange && LineOfSight(target))
        {
            SpellCast(engine, target);
        }
        else if (Mathf.Abs((parent.transform.position - target.transform.position).magnitude) < engine.lightingRenderer.playerLightRadius)
        {
            get_path_to(engine, parent, target);//pathfind and then move
        }

        new WaitAction().Perform(engine);

    }


    public virtual void SpellCast(GameManager engine, Entity target)
    {
        GridMap Grid = engine.Grid;
        if (!casting)
        {
            casting = true;
            currSprite.sprite = Sprites[0];
            toCast = Grid.WorldToTile(target.transform.position);
        }
        else if (casting)
        {
            foreach (Tile t in Grid.GetNeighbouringTiles(toCast))
            {
                if (Grid.GetActorAt(t.tilePostion))
                {
                    Grid.GetActorAt(t.tilePostion).fighter.hp -= parent.fighter.power;
                }
                engine.effectsManager.InstantiateEffect(Database._instance.GetObjectInEffects(spellEffect), t.tilePostion);
            }
            casting = false;
            currSprite.sprite = Sprites[1];
        }
    }

}
