using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    NetworkManagerCommune manager { get; set; }

    public void Start()
    {
        manager = FindObjectOfType<NetworkManagerCommune>();
    }


    public void Create()
    {
        //SceneManager.LoadScene("Town");
        manager.StartHost();
    }

    public void Join()
    {
        manager.StartClient();
        manager.networkAddress = "localhost";
    }

    public void Options()
    {
        Debug.Log("Opening options");
    }

    public void Quit()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }
}
