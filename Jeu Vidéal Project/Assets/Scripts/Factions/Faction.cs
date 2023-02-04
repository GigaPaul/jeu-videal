using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Faction : MonoBehaviour
{
    public int Id { get; set; }
    public string Label;
    public Material Material;

    private void Awake()
    {
        Id = Globals.Factions.Count;
        Globals.Factions.Add(this);
    }

    private void FixedUpdate()
    {
        if(Material != null)
        {
            List<Pawn> members = FindObjectsOfType<Pawn>().Where(i => i.Faction == this).ToList();

            foreach(Pawn pawn in members)
            {
                List<Renderer> renderers = pawn.Model.GetComponentsInChildren<Renderer>().Where(i => i.material != Material).ToList();

                foreach (Renderer renderer in renderers)
                {
                    renderer.material = Material;
                }
            }
        }
    }

    public bool IsPlayable()
    {
        return Globals.Factions.FirstOrDefault(i => i.Label == "Player") == this;
    }
}
