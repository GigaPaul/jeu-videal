using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    public int Id { get; set; }
    public string Label;
    public Culture _Culture;
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
    FlockManager WarBand { get; set; }





    private void Awake()
    {
        Id = Globals.Settlements.Count;
        Globals.Settlements.Add(this);
        ResourceStock = 0;
        VillagerCost = 50;
    }





    // Start is called before the first frame update
    void Start()
    {
        DrawRadius();

        InvokeRepeating(nameof(BalanceJobs), 0, 1);
        //InvokeRepeating(nameof(CheckResources), 0, 1);
    }





    public void FormFlock()
    {
        if (IsDeserted())
        {
            return;
        }

        if(WarBand != null)
        {
            return;
        }

        List<Pawn> warriors = GetVillagersWithJob(typeof(SmartGuard));

        if (!warriors.Any())
        {
            return;
        }


        GameObject flockGO = Resources.Load("Prefabs/FlockManager") as GameObject;
        FlockManager flockPrefab = flockGO.GetComponent<FlockManager>();

        WarBand = Instantiate(flockPrefab, GetRandomPoint(), Quaternion.identity);


        WarBand.AddMember(warriors);
        WarBand.Commander = WarBand.Members.First();
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
            else if (GetVillagersWithJob(typeof(SmartWorker)).Count > 0)
            {
                pawnToPromote = GetVillagersWithJob(typeof(SmartWorker)).First();
            }
            else if (GetVillagersWithJob(typeof(SmartTrader)).Count > 0)
            {
                pawnToPromote = GetVillagersWithJob(typeof(SmartTrader)).First();
            }
            else if (GetVillagersWithJob(typeof(SmartGuard)).Count > 0)
            {
                pawnToPromote = GetVillagersWithJob(typeof(SmartGuard)).First();
            }


            if (pawnToPromote != null)
            {
                if (!HasEnoughInnkeepers())
                {
                    pawnToPromote.AssignTo(typeof(SmartInnkeeper));
                }
                else if (!HasEnoughWarriors())
                {
                    pawnToPromote.AssignTo(typeof(SmartGuard));
                    if(WarBand != null)
                    {
                        WarBand.AddMember(pawnToPromote);
                    }
                }
                else if (!HasEnoughTraders())
                {
                    pawnToPromote.AssignTo(typeof(SmartTrader));
                }
                else
                {
                    pawnToPromote.AssignTo(typeof(SmartWorker));
                }
            }
        }




        FormFlock();
    }


    public void CreateVillager()
    {
        GameObject villagerPrefab = Resources.Load("Prefabs/Pawn") as GameObject;
        Pawn newPawn = villagerPrefab.GetComponent<Pawn>();

        newPawn.GetComponent<PawnAttributes>()._Culture = _Culture;
        newPawn.Faction = Faction;
        newPawn.Settlement = this;

        Instantiate(newPawn, GetRandomPoint(), Quaternion.identity);
    }


    private void CheckResources()
    {
        float maxVillagers = 20;

        if(ResourceStock >= VillagerCost && GetVillagers().Count <= maxVillagers)
        {
            Debug.Log($"Spawning new villager in {Label}");
            ResourceStock -= VillagerCost;
            CreateVillager();
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
        return GetVillagers().Where(i => i.GetComponent<SmartPawn>() && i.GetComponent<SmartUnemployed>() == null).ToList();
    }

    public List<Pawn> GetActiveCivilians()
    {
        return GetActives().Where(i => !GetVillagersWithJob(typeof(SmartGuard)).Contains(i)).ToList();
    }

    public List<Pawn> GetVillagersWithJob(Type type)
    {
        return GetActives().Where(i => i.GetComponent(type)).ToList();
    }

    public List<Pawn> GetUnemployed()
    {
        return GetVillagers().Where(i => !GetActives().Contains(i)).ToList();
    }

    public Vector3 GetRandomPoint(float percentage = 1)
    {
        float angle = UnityEngine.Random.Range(0, Mathf.PI * 2);
        float radius = Mathf.Sqrt(UnityEngine.Random.Range(0f, 1)) * Size * percentage;
        Vector3 randomPlace = new (Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

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
        return GetVillagersWithJob(typeof(SmartInnkeeper)).Count >= GetMinInnkeepers();
    }

    public bool HasEnoughWarriors()
    {
        return GetVillagersWithJob(typeof(SmartGuard)).Count >= GetMinWarriors();
    }

    public bool HasEnoughTraders()
    {
        return GetVillagersWithJob(typeof(SmartTrader)).Count >= GetMinTraders();
    }

    public bool IsDeserted()
    {
        return GetVillagers().Count == 0;
    }
}
