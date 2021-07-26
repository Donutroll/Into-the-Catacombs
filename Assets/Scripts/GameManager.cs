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

    public GameObject player;

    

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this) //prevents additional GameManager across scenes
        {
            Destroy(gameObject);
            Debug.LogError("GameManager already exists");
        }
       
        player = Instantiate(player);
        player.GetComponent<Actor>().fighter.parent = player.GetComponent<Actor>(); 
        eventManager = new MainGameEvent(this, player);
        uiManager.engine = this;
        uiManager.player = player;
        
    }


    private void Start()
    {
        Grid.Entities.Add(player.GetComponent<Entity>());
        Grid.GenerateMap(player, 20, 3 , 6 , 3 , 7 , 10,5);
    }


    void Update()
    {
        if (inputManager.inputList.Count > 0)
            eventManager.HandleEvent();
    }

    public GameObject CreateObject(GameObject gameObject)
    {
        return Instantiate(gameObject);
    }


    public void DestroyObject(GameObject gameObject)
    {
       Destroy(gameObject);
    }



}
