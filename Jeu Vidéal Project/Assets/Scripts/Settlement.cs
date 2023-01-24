using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    public int Id { get; set; }
    public string Label;

    private void Awake()
    {
        Id = Globals.Settlements.Count;
        Globals.Settlements.Add(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
