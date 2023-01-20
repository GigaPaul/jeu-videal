using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Truc()
    {
        return;
    }

    public void SelectBuilding(int id)
    {
        GameObject prefab = null;

        if (id == 0)
        {
            prefab = Resources.Load("Prefabs/SM_Bld_Shop_Corner_01") as GameObject;
        }
        else if(id == 1)
        {
            prefab = Resources.Load("Prefabs/SM_Bld_Church_01") as GameObject;
        }
        else
        {
            Debug.Log("Aucun bâtiment trouvé avec cette id : "+id);
        }

        if(prefab != null)
        {
            GameObject instantiatedPrefab = Instantiate(prefab, Vector3.zero, Quaternion.identity);
            Building buildingToBlueprint = instantiatedPrefab.GetComponent<Building>();
            buildingToBlueprint.ToBlueprint();
        }
    }
}
