using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcEnemy : WarlockEnemy
{
    public int hitRadius = 1;
    protected override void Start()
    {
        base.Start();
    }

    public override int TakeTurn(GameManager engine)
    {
        return base.TakeTurn(engine);
    }

    public override void SpellCast(GameManager engine, Entity target)
    {
        if (!casting)
        {
            casting = true;
            currSprite.sprite = Sprites[0];
        }
        else if (casting)
        {
            GridMap Grid = engine.Grid;
            Tile toCast = Grid.WorldToTile(transform.position);

            Rooms.SquareRoom castSpace = new Rooms.SquareRoom(engine.Grid, toCast.gridX - hitRadius, toCast.gridY - hitRadius, hitRadius * 2);
            castSpace.RoomTiles.Remove(toCast);

            foreach (Tile t in castSpace.RoomTiles)
            {
                if (Grid.GetActorAt(t.tilePostion))
                {
                    Grid.GetActorAt(t.tilePostion).fighter.hp -= parent.fighter.power;
                }
                engine.effectsManager.InstantiateEffect(parent.HitEffect, t.tilePostion);
            }
            casting = false;
            currSprite.sprite = Sprites[1];
        }
    }

}
