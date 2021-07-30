using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneManager : MonoBehaviour
{
    public GameObject inventory;
    public GameObject inputManager;
    public GameObject playerinfo;
    public GameObject startText;
    public GameObject Cam;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(Cam);
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * 20000);
            playerinfo.SetActive(true);
            inventory.SetActive(true);
            inputManager.SetActive(true);
            Destroy(this.gameObject, 1f);
        }
    }
}
