using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public Rigidbody rb { get; set; }
    public List<Module> modules = new();





    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }





    // Methods
    void ChangeLayerTo(string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);

        gameObject.layer = layer;
        foreach (Transform child in transform)
        {
            child.gameObject.layer = layer;
        }
    }

    public void Place(Transform trans)
    {
        transform.position = trans.position;
        transform.rotation = trans.rotation;
        ChangeLayerTo("Default");
        rb.isKinematic = false;
    }

    public void ToBlueprint()
    {
        if (Globals.BuildingBlueprint != null)
            Destroy(Globals.BuildingBlueprint.gameObject);

        Globals.BuildingBlueprint = this;
        ChangeLayerTo("No Collision");

        rb.isKinematic = true;

    }





    // Booleans
    public bool IsBlueprint()
    {
        return Globals.BuildingBlueprint == this;
    }
}
