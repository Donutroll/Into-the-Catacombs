using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class GridManage : MonoBehaviour
{
    public static GridManage instance;

    public GameObject board;
    public CreateGrid grid;
    public PathFinding pathFinding;
    public MovingEntity[] Entities;
    public GameObject player;
    PlayerManager playerManager;


    private void Awake()
    {
        instance = this;
        grid = board.GetComponent<CreateGrid>();
        pathFinding = board.GetComponent<PathFinding>();
        playerManager = player.GetComponent<PlayerManager>();
        playerManager.input += EnemyTurn;

        foreach ( MovingEntity E in Entities)
        {
            E.entityPosition = grid.GetNodeFromWorldPosition(E.transform.position);
        }
    }

    private void Start()
    {
        
    }

    private void Update()
    {
       
    }



    void EnemyTurn()
    {
        for (int actor = 0; actor < Entities.Length; ++actor)
        {
            MovingEntity curActor = Entities[actor];
            curActor.entityPosition.neighbourNodes = grid.GetNeighbouringNodes(curActor.entityPosition);

            

            if (curActor.energy >= 10)
            {
                Debug.Log(Entities[actor].name + " : is acting"); //send to delegate
                Entities[actor].energy -= 10;
                switch ((int)curActor.GetAction())
                {
                    case 0:
                        List<Tile> finalPath = pathFinding.GetPath(curActor.transform, player.transform);
                        if (finalPath != null)
                        {
                            StartCoroutine(MoveActor(curActor, finalPath[0]));
                        }
                        else
                            Debug.Log("no path found");
                        break;
                    case 1:
                        // attack act
                        break;
                    default:
                        Debug.Log(Entities[actor].name + " : no action avaiable");
                        break;

                }
            }
            else
            {
                Entities[actor].energy += Entities[actor].energyPerTurn;
                continue;
            }
        }
    }

    IEnumerator MoveActor(MovingEntity entitytoMove, Tile destination)
    {
        yield return new WaitForSeconds(.5f);
        entitytoMove.transform.position = destination.worldPos;
        entitytoMove.entityPosition = destination;

    }

}
*/