using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Action
{
    public string Label { get; set; }
    public float CastingTime { get; set; } = 3;
    public Pawn Actor { get; set; }
    #nullable enable
    public Transform? Target { get; set; }

    public Func<Task>? Result { get; set; }
    #nullable disable

    public async Task Perform()
    {
        if(Result != null)
        {
            await Result();
        }
    }
}
