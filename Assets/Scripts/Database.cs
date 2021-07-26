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

    public List<GameObject> enemyDb;

    public List<GameObject> effectsDb;

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
    public GameObject GetObjectInEnemy( string objectName)
    {
        return GetObject(enemyDb, objectName);
    }


    public GameObject GetObjectInEffects(string objectName)
    {
        return GetObject(effectsDb, objectName);
    }



}
