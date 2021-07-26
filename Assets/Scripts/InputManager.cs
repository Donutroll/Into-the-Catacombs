using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameManager engine;
    public GameObject SelectionBorder;
    public List<KeyCode> inputList = new List<KeyCode>(); 
    public Vector3 mousePos;

    private void Start()
    {
        mousePos = Input.mousePosition;
    }
    private void OnGUI()//stores information when keyboard/mouse are pressed
    {
        Event input = Event.current;
        if (input.isKey && input.type == EventType.KeyUp)
        {
            inputList.Add(input.keyCode);
        }

    }

    private void Update()
    {
        mousePos = Input.mousePosition;
    }
}



