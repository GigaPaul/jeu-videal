using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButton : MonoBehaviour
{
    NetworkManagerVideal manager { get; set; }

    public void Start()
    {
        manager = FindObjectOfType<NetworkManagerVideal>();
    }


    public void Create()
    {
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
        Application.Quit();
    }
}
