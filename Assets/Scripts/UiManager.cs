    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class UiManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Canvas gameCanvas;
    public Text playerHeatlh;
    public Text coinCount;
    public GameObject PauseMenu;

    public GameManager engine;

    public GameObject player;
    public GameObject PlayerInventory;
    public List<GameObject> InventorySlots;
    public GameObject SelectionBorder;

    int coins = 0;

    private void Start()
    {
        player = engine.player;
        UpdatePlayerHealth(player.GetComponent<Actor>().fighter);
        player.GetComponent<Actor>().inventory.OnInventoryChange += UpdatePlayerInventory;
        player.GetComponent<Actor>().PlayerDamage += UpdatePlayerHealth;
    }
    private void Update()
    {

    }

    public void UpdatePlayerHealth(Fighter player)
    {
        playerHeatlh.text = "Hp: " + player.hp + "/" + player.maxHp;
        if (player.hp <= 0)
        {
              engine.eventManager = new GameOverEvent(engine);
            Instantiate(PauseMenu);
        }
    }

    public void UpdateCoinCount(int amount=1)
    {
        coins += amount;
        coinCount.text = "Coin " + coins; 
    }


    
    public void UpdatePlayerInventory()
    {
        List<Item> Items = player.GetComponent<Actor>().inventory.items;
        for(int i = 0; i < InventorySlots.Count;++i)
            InventorySlots[i].GetComponent<Image>().gameObject.SetActive(false);

        for (int i = 0; i < Items.Count; ++i)
        {
            Image slotImage = InventorySlots[i].GetComponent<Image>();
            if (Items[i] != null)
            {
                slotImage.sprite = Database._instance.GetObjectInItems(Items[i].type).GetComponentInChildren<SpriteRenderer>().sprite;
                slotImage.gameObject.SetActive(true);
            }


        }

    }


}
