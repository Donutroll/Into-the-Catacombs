using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Entity : MonoBehaviour //Generic class holding all members of the game
{
    public delegate void OnDeath(Entity entity);
    public delegate void OnDamage(Fighter fighter);
    public event OnDeath Death;

    public bool blockMove = true;
    public string type;

    public Fighter fighter = null;

    public GameManager engine;

    public void EntityDeath(Entity entity)
    {
        Debug.Log("Evoked");
       Death?.Invoke(entity);// Event to GridMap.OnEntityDeath
    }

    public void selfDistruct()
    {
        Destroy(this.gameObject);
    }

    
    public void Move(int dx, int dy)
    {
        transform.position += new Vector3(dx, dy, 0);
    }

    public float DistanceBetween(Vector3 otherPosition)
    {
        return Mathf.Sqrt( Mathf.Pow((this.transform.position.x - otherPosition.x), 2) + Mathf.Pow((this.transform.position.y - otherPosition.y), 2) ); //gets distance between two entities
    }






}





    




