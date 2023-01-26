using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settlement : MonoBehaviour
{
    public int Id { get; set; }
    public string Label;
    public int Size = 10;
    public LineRenderer CircleRenderer;
    public bool IsBeingDebugged = false;

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

    // Update is called once per frame
    void Update()
    {
        
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
