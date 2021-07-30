    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
public class UiManager : MonoBehaviour
{

    public Canvas gameCanvas;
    public Text playerHeatlh;
    public Text coinCount;
    public Text floorCount;
    public GameObject GameOverMenu;
    public GameObject PauseMenu;
    public GameManager engine;

    public GameObject Pause;

    public GameObject player;
    public GameObject PlayerInventory;
    public List<GameObject> InventorySlots;
    public GameObject SelectionBorder;

    int floor = 0;
    int coins = 0;

    private void Start()
    {
        player = engine.player;
        UpdatePlayerHealth(player.GetComponent<Actor>().fighter);
        player.GetComponent<Actor>().inventory.OnInventoryChange += UpdatePlayerInventory;
        player.GetComponent<Actor>().PlayerDamage += UpdatePlayerHealth;
    }

    public void UpdatePlayerHealth(Fighter player) //updates player health and toggles game over when needed
    {
        playerHeatlh.text = "Hp: " + "\n" + player.hp + "/" + player.maxHp;
        if (player.hp <= 0)
        {
            engine.eventManager = new GameOverEvent(engine);
            GameOverMenu = Instantiate(GameOverMenu);
            GameOverMenu.transform.SetParent(GameObject.Find("Canvas").transform);
            GameOverMenu.GetComponentInChildren<Text>().text += " " + coins;
            GameOverMenu.GetComponentInChildren<Button>().onClick.AddListener(ResetScene);
        }
    }

    public void UpdateCoinCount(int amount=1) //updates coin count
    {
        coins += amount;
        coinCount.text = "Coin " + "\n" + coins; 
    }

    public void UpdateFloorCount()//updates floor count
    {
        floor += 1;
        floorCount.text = "Floor" + "\n" + floor;
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

    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CreatePause()
    {
        Pause = Instantiate(PauseMenu);
        Pause.transform.SetParent(GameObject.Find("Canvas").transform);
        Pause.transform.Find("Resume").GetComponent<Button>().onClick.AddListener(ExitPause);
        Pause.transform.Find("Menu").GetComponent<Button>().onClick.AddListener(ResetScene);
    }
    public void ExitPause()
    {
        ((PauseScreenEvent)engine.eventManager).OnExit();
        Destroy(Pause);
    }
}
