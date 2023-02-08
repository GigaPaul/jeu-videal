using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Faction : MonoBehaviour
{
    public int Id { get; set; }
    public string Label;
    public Material Material;
    public List<FactionRelation> Relationships = new();





    private void Awake()
    {
        Id = Globals.Factions.Count;
        Globals.Factions.Add(this);

        List<Faction> factions = FindObjectsOfType<Faction>().Where(i => i != this).ToList();

        foreach(Faction faction in factions)
        {
            FactionRelation relationship = new()
            {
                Faction = faction
            };

            Relationships.Add(relationship);
        }
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





    public int GetRelationTo(Faction faction)
    {
        return Relationships.FirstOrDefault(i => i.Faction == faction).Opinion;
    }

    public void Ally(Faction faction)
    {
        Relationships.FirstOrDefault(i => i.Faction == faction).Opinion = 100;
    }

    public void DeclareWarTo(Faction faction)
    {
        Relationships.FirstOrDefault(i => i.Faction == faction).Opinion = -100;
    }

    public void MakePeaceWith(Faction faction)
    {
        if(IsAtWarWith(faction))
        {
            Relationships.FirstOrDefault(i => i.Faction == faction).Opinion = 0;
        }
    }





    public bool IsPlayable()
    {
        return Globals.Factions.FirstOrDefault(i => i.Label == "Player") == this;
    }

    public bool IsAtWarWith(Faction faction)
    {
        return GetRelationTo(faction) == -100;
    }

    public bool IsHostileTo(Faction faction)
    {
        return GetRelationTo(faction) < 0;
    }

    public bool IsFavorableTo(Faction faction)
    {
        return !IsHostileTo(faction);
    }

    public bool IsAlliedTo(Faction faction)
    {
        return GetRelationTo(faction) == 100;
    }
}
