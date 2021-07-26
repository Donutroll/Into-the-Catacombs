using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Entity //An item is an Entity that does not act
{
    public Consumable consumable = null;
    public void Start()
    {
        consumable.parent = this;
    }
}
