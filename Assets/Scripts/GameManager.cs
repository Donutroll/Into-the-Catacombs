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

    public List<Entity> testEntities;

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
        player.GetComponent<Actor>().engine = this;
        eventManager = new MainGameEvent(this, player);

        uiManager.engine = this;
        lightingRenderer.engine = this;



    }


    private void Start()
    {

        Grid.Entities.Add(player.GetComponent<Entity>());
        Grid.TileMapToGridMap();//creates a 2d array based on Tile placements in Unity
    }


    void Update()
    {
        if (inputManager.inputList.Count > 0)//inputManager has its own update function that will cache inputs into a list
        {
            eventManager.HandleEvent();
            lightingRenderer.RenderLight(lightingRenderer.playerLightRadius, player.GetComponent<Actor>());
        }
    }





    public GameObject CreateObject(GameObject gameObject)//helper functions for non monobehaviours
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
