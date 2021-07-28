using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class GameManager : MonoBehaviour
{
    public static GameManager _instance; //only use this if ABSOLUTELY NECESSARY

    public GridMap Grid;
    public EventManager eventManager;
    public UiManager uiManager;
    public InputManager inputManager;
    public EffectsManager effectsManager;
    public LightingRenderer lightingRenderer;

    public GameObject player;
    public bool playerdead;


    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this) //prevents additional GameManager across scenes
        {
            Destroy(gameObject);
            Debug.LogError("GameManager already exists");
        }

        player = Instantiate(player, new Vector3(0.5f,0.5f,0), Quaternion.identity);

        eventManager = new MainGameEvent(this, player);

        uiManager.engine = this;
        lightingRenderer.engine = this;

    }


    private void Start()
    {
        Grid.Entities.Add(player.GetComponent<Entity>());
        Grid.TileMapToGridMap();

    }


    void Update()
    {
        if (inputManager.inputList.Count > 0)
        {
            eventManager.HandleEvent();
            lightingRenderer.RenderLight(lightingRenderer.playerLightRadius, player.GetComponent<Actor>());
        }
    }

    /* if (Grid.Actors[currentActor].type == "Player" && player.GetComponent<Actor>().fighter.Energy >= Fighter.actionCost)
 {
     if (inputManager.inputList.Count > 0)
     {
         eventManager.HandleEvent();
         player.GetComponent<Actor>().fighter.Energy -= Fighter.actionCost;
         currentActor = (currentActor + 1) % Grid.Actors.Count;
     } 
 }
 else if(Grid.Actors[currentActor].type == "Player" && player.GetComponent<Actor>().fighter.Energy < Fighter.actionCost)
 {
     player.GetComponent<Actor>().fighter.Energy += player.GetComponent<Actor>().fighter.recoverySpeed;
     currentActor = (currentActor + 1) % Grid.Actors.Count;
 }
 else
 {
     Grid.Actors[currentActor].ai.Perform(this, player.GetComponent<Actor>());
     currentActor = (currentActor + 1) % Grid.Actors.Count;
 } */




    public GameObject CreateObject(GameObject gameObject)
    {
        return Instantiate(gameObject);
    }
    public void DestroyObject(GameObject gameObject)
    {
        Destroy(gameObject);
    }

    public static float VectorDistance(GameObject a, GameObject b)
    {
        return Mathf.Abs((a.transform.position - b.transform.position).magnitude);
    }


}
