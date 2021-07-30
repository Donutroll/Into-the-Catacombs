using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Database : MonoBehaviour
{
    public static Database _instance; //database singleton, grants access everywhere

    public void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this) 
        {
            Destroy(gameObject);
            Debug.LogError("Database already exists");
        }
    }

    public List<GameObject> itemDb;

    public List<GameObject> actorsDb;

    public List<GameObject> effectsDb;

    public List<GameObject> interactableDb;

    public GameObject GetObject( List<GameObject> database, string objectName)
    {
        foreach( GameObject g in database)
        {
            if (g.name == objectName)
                return g;
        }

        Debug.LogError(objectName + " was not found in database");
        return null;
    }

    public GameObject GetObjectInItems( string objectName)
    {
        return GetObject(itemDb, objectName);
    }
    public GameObject GetObjectInActors( string objectName)
    {
        return GetObject(actorsDb, objectName);
    }


    public GameObject GetObjectInEffects(string objectName)
    {
        return GetObject(effectsDb, objectName);
    }

    public GameObject GetRandomItem()
    {
        int chance = Random.Range(0, 20);
        string name;
        if (chance <= 2)
            name = "Fire Scroll";
        else if (chance > 2 && chance <= 6)
            name = "Lightning Scroll";
        else
            name = "Healing Potion";

        return GetObjectInItems(name);
    }


    public GameObject GetRandomEnemy()
    {
        int chance = Random.Range(0, 30);
        string name;
        if (chance <= 2)
            name = "Orc";
        else if (chance > 2 && chance < 6)
            name = "Warlock";
        else if (chance >= 6 && chance < 15)
            name = "Goblin";
        else if (chance >= 15 && chance < 20)
            name = "SwordGoblin";
        else
            name = "Slime";

        return GetObjectInActors(name);
    }

    public GameObject GetObjectFromInteractable(string objectName)
    {
        return GetObject(interactableDb, objectName);
    }


}
