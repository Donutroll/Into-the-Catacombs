using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : Actor
{
    void Start()
    {
        fighter.parent = this;
    }

    public List<GameObject> possibleItems = new List<GameObject>();

    public virtual GameObject OnTrigger()
    {
        GameObject randomItem = possibleItems[Random.Range(0, possibleItems.Count - 1)];
        return randomItem;
    }
}
