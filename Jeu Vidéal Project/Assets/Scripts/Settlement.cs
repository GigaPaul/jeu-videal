using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    public int Id { get; set; }
    public string Label;
    public Faction Faction;
    public int Size = 10;
    public LineRenderer CircleRenderer;
    public bool IsBeingDebugged = false;

    public Transform Storage;
    public Transform Inn;
    public List<Transform> WorkingStations = new();
    public List<Transform> Patrol = new();
    public int ResourceStock;
    public int VillagerCost { get; set; }

    private void Awake()
    {
        Id = Globals.Settlements.Count;
        Globals.Settlements.Add(this);
        ResourceStock = 0;
        VillagerCost = 50;

        InvokeRepeating(nameof(BalanceJobs), 0, 1);
        InvokeRepeating(nameof(CheckResources), 0, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        DrawRadius();
    }

    public void BalanceJobs()
    {
        if (IsDeserted())
        {
            return;
        }





        while (!HasEnoughInnkeepers() || !HasEnoughWarriors() || !HasEnoughTraders() || GetUnemployed().Count > 0)
        {
            Pawn pawnToPromote = null;


            if (GetUnemployed().Count > 0)
            {
                pawnToPromote = GetUnemployed().First();
            }
            else if (GetVillagersWithJob("worker").Count > 0)
            {
                pawnToPromote = GetVillagersWithJob("worker").First();
            }
            else if (GetVillagersWithJob("trader").Count > 0)
            {
                pawnToPromote = GetVillagersWithJob("trader").First();
            }
            else if (GetVillagersWithJob("warrior").Count > 0)
            {
                pawnToPromote = GetVillagersWithJob("warrior").First();
            }


            if (pawnToPromote != null)
            {
                if (!HasEnoughInnkeepers())
                {
                    pawnToPromote.AssignTo("innkeeper");
                }
                else if (!HasEnoughWarriors())
                {
                    pawnToPromote.AssignTo("warrior");
                }
                else if (!HasEnoughTraders())
                {
                    pawnToPromote.AssignTo("trader");
                }
                else
                {
                    pawnToPromote.AssignTo("worker");
                }
            }
        }
    }

    private void CheckResources()
    {
        if(ResourceStock >= VillagerCost)
        {
            Debug.Log($"Spawning new villager in {Label}");
            ResourceStock -= VillagerCost;
            GameObject villagerPrefab = Resources.Load("Prefabs/Pawn") as GameObject;
            Pawn newPawn = villagerPrefab.GetComponent<Pawn>();

            newPawn.Faction = Faction;
            newPawn.Settlement = this;

            Instantiate(newPawn, GetRandomPoint(), Quaternion.identity);
        }
    }

    void DrawRadius()
    {
        int steps = 100;
        CircleRenderer.positionCount = steps;
        for (int currentStep = 0; currentStep < steps; currentStep++)
        {
            float CircumferenceProgress = (float)currentStep / steps;
            float CurrentRadian = CircumferenceProgress * 2 * Mathf.PI;
            float xScaled = Mathf.Cos(CurrentRadian);
            float zScaled = Mathf.Sin(CurrentRadian);
            float x = xScaled * Size;
            float z = zScaled * Size;

            Vector3 CurrentPosition = new(x, 0.25f, z);

            CircleRenderer.SetPosition(currentStep, CurrentPosition);
        }
    }





    public List<Pawn> GetVillagers()
    {
        return FindObjectsOfType<Pawn>().Where(i => i.Settlement == this).ToList();
    }

    public List<Pawn> GetActives()
    {
        return GetVillagers().Where(i => i.Occupations.Any()).ToList();
    }

    public List<Pawn> GetActiveCivilians()
    {
        return GetActives().Where(i => !GetVillagersWithJob("warrior").Contains(i)).ToList();
    }

    public List<Pawn> GetVillagersWithJob(string occupation)
    {
        return GetActives().Where(i => i.Occupations.Contains(occupation)).ToList();
    }

    public List<Pawn> GetUnemployed()
    {
        return GetVillagers().Where(i => !GetActives().Contains(i)).ToList();
    }

    public Vector3 GetRandomPoint(float percentage = 1)
    {
        float angle = Random.Range(0, Mathf.PI * 2);
        float radius = Mathf.Sqrt(Random.Range(0f, 1)) * Size * percentage;
        Vector3 randomPlace = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

        return transform.position + randomPlace;
    }





    public int GetMinInnkeepers()
    {
        return GetVillagers().Count > 0 ? 1 : 0;
    }

    public int GetMinWarriors()
    {
        return Mathf.CeilToInt( (GetVillagers().Count - GetMinInnkeepers()) / 3f);
    }

    public int GetMinTraders()
    {
        return Mathf.CeilToInt( (GetVillagers().Count - (GetMinInnkeepers() + GetMinWarriors())) / 4f);
    }





    public bool HasEnoughInnkeepers()
    {
        return GetVillagersWithJob("innkeeper").Count >= GetMinInnkeepers();
    }

    public bool HasEnoughWarriors()
    {
        return GetVillagersWithJob("warrior").Count >= GetMinWarriors();
    }

    public bool HasEnoughTraders()
    {
        return GetVillagersWithJob("trader").Count >= GetMinTraders();
    }

    public bool IsDeserted()
    {
        return GetVillagers().Count == 0;
    }
}
