using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Faction : MonoBehaviour
{
    public int Id { get; set; }
    public string Label;

    private void Awake()
    {
        Id = Globals.Factions.Count;
        Globals.Factions.Add(this);
    }

    private void Start()
    {
        
    }

    private void Update()
    {

    }

    public bool IsPlayable()
    {
        return Globals.Factions.FirstOrDefault(i => i.Label == "Player") == this;
    }
}
