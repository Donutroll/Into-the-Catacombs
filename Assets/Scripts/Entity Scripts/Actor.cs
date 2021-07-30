using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class Actor : Entity //An Actor is an Entity that acts
{
    public event OnDamage PlayerDamage;
    public GameObject HitEffect;

    public BaseAi ai = null;
    public Inventory inventory = null;

    private void Start()
    {
        inventory.parent = this;
        fighter.parent = this;

        ai = GetComponent<BaseAi>();
    }

    public bool is_alive()
    {
        return this.ai != null ? true : false; //an actor is alive if it can perform actions
    }

    public void ActorDamaged(Fighter fighter, int damage)
    {
        Debug.Log(fighter.parent.type + " takes " + damage + "!");
        if (type == "Player")
            PlayerDamage?.Invoke(fighter);

        GameObject DamagePopup = Database._instance.GetObjectInEffects("FloatingUi");
        DamagePopup.GetComponentInChildren<TextMeshPro>().text = damage > 0 ? "+" + damage.ToString() : damage.ToString();
        DamagePopup = Instantiate(DamagePopup, this.transform.position, Quaternion.identity); //passed twice because instantiate creates an individual instance
        Destroy(DamagePopup, 1f);
    }


}

