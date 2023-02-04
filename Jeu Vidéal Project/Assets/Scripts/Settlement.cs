using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    public int Id { get; set; }
    public string Label;
    public int Size = 10;
    public LineRenderer CircleRenderer;
    public bool IsBeingDebugged = false;

    public Transform Inn;
    public List<Transform> WorkingStations = new();
    public List<Transform> Patrol = new();

    private void Awake()
    {
        Id = Globals.Settlements.Count;
        Globals.Settlements.Add(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        DrawRadius();
    }

    private void FixedUpdate()
    {
        List<Pawn> pawns = FindObjectsOfType<Pawn>().Where(i => i.Settlement == this).ToList();

        if (pawns.Count > 0)
        {
            List<Pawn> unemployed = pawns.Where(i => !i.Occupations.Any()).ToList();
            List<Pawn> actives = pawns.Where(i => !unemployed.Contains(i)).ToList();

            List<Pawn> warriors = actives.Where(i => i.Occupations.Contains("warrior")).ToList();
            List<Pawn> traders = actives.Where(i => i.Occupations.Contains("trader")).ToList();
            List<Pawn> workers = actives.Where(i => i.Occupations.Contains("worker")).ToList();
            List<Pawn> innkeepers = actives.Where(i => i.Occupations.Contains("innkeeper")).ToList();

            int minInnkeeper = 1;
            int minWarriors = Mathf.CeilToInt((pawns.Count - minInnkeeper) / 3f);
            int minTraders = Mathf.CeilToInt((pawns.Count - minInnkeeper - minWarriors) / 4f);

            while (unemployed.Any())
            {
                Pawn thisUnemployed = unemployed.First();

                if(innkeepers.Count < minInnkeeper)
                {
                    innkeepers.Add(thisUnemployed);
                    thisUnemployed.AssignTo("innkeeper");
                }
                else if(warriors.Count < minWarriors)
                {
                    warriors.Add(thisUnemployed);
                    thisUnemployed.AssignTo("warrior");
                }
                else if(traders.Count < minTraders)
                {
                    traders.Add(thisUnemployed);
                    thisUnemployed.AssignTo("trader");
                }
                else
                {
                    workers.Add(thisUnemployed);
                    thisUnemployed.AssignTo("worker");
                }

                unemployed.Remove(thisUnemployed);
            }
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
}
