using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Faction : MonoBehaviour
{
    public string Id;
    public string Label;
    public Material Material;
    public List<FactionRelation> Relationships = new();
    public bool IsBeingDebugged;





    private void Awake()
    {
        List<Faction> factions = FindObjectsOfType<Faction>().Where(i => i != this).ToList();

        foreach(Faction faction in factions)
        {
            FactionRelation relationship = new()
            {
                Faction = faction
            };

            if(Label == "Abourg" && faction.Label == "Bescheim" || Label == "Bescheim" && faction.Label == "Abourg")
            {
                relationship.Opinion = -100;
            }

            Relationships.Add(relationship);
        }
    }





    private void Start()
    {
        InvokeRepeating(nameof(CheckMaterials), 0, 0.5f);
        //InvokeRepeating(nameof(ManageDiplomacy), 0, 5);
    }




    private void CheckMaterials()
    {
        if (Material != null)
        {
            List<Pawn> members = FindObjectsOfType<Pawn>().Where(i => i.Faction == this).ToList();

            foreach (Pawn pawn in members)
            {
                foreach (Renderer renderer in pawn.Model.Renderers)
                {
                    renderer.material = Material;
                }
            }
        }
    }





    private void ManageDiplomacy()
    {
        List<Faction> factions = FindObjectsOfType<Faction>().Where(i => i != this).ToList();
        float aggravatingOdds = 1f / 3f;
        float improvingOdds = 1f / 3f;
        float warOdds = 1f / 5f;
        float peaceOdds = 1f / 5f;

        foreach (Faction faction in factions)
        {
            FactionRelation relation = Relationships.FirstOrDefault(i => i.Faction == faction);

            float relationChangement = Random.Range(0f, 1);

            if(0 <= relationChangement && relationChangement <= aggravatingOdds)
            {
                if (relation.Opinion > - 100)
                {
                    relation.Opinion -= 10;
                    //Debug.Log($"Les relations entre {Label} et {faction.Label} ont descendu de 10 points : {relation.Opinion}");
                }
            }
            else if (aggravatingOdds < relationChangement && relationChangement <= aggravatingOdds + improvingOdds)
            {
                if (relation.Opinion < 100)
                {
                    relation.Opinion += 10;
                    //Debug.Log($"Les relations entre {Label} et {faction.Label} ont augmenté de 10 points : {relation.Opinion}");
                }
            }



            float statusChangement = Random.Range(0f, 1);
            if (!IsAtWarWith(faction) && relation.Opinion <= -20)
            {
                if(0 <= statusChangement && statusChangement <= warOdds)
                {
                    DeclareWarTo(faction);
                    //Debug.Log($"{Label} a déclaré la guerre à {faction.Label}!");
                }
            }
            else if(IsAtWarWith(faction) && relation.Opinion >= 20)
            {
                if (0 <= statusChangement && statusChangement <= peaceOdds)
                {
                    MakePeaceWith(faction);
                    //Debug.Log($"{Label} a fait la paix avec {faction.Label}!");
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
        faction.Relationships.FirstOrDefault(i => i.Faction == this).Opinion = -100;
    }

    public void MakePeaceWith(Faction faction)
    {
        if(IsAtWarWith(faction))
        {
            Relationships.FirstOrDefault(i => i.Faction == faction).Opinion = 0;
            faction.Relationships.FirstOrDefault(i => i.Faction == this).Opinion = 0;
        }
    }





    public bool IsPlayable()
    {
        return FindObjectsOfType<Faction>().FirstOrDefault(i => i.Label == "Player") == this;
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
