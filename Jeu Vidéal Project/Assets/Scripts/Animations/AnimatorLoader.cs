using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatorLoader : MonoBehaviour
{
    public AnimatorController Controller;
    private List<int> Layers = new();

    private void Awake()
    {
        //GenerateLayers();
    }


    private void GenerateLayers()
    {
        AnimatorControllerLayer layerToCopy = null;

        foreach (AnimatorControllerLayer layer in Controller.layers)
        {
            if(layer.name == "Animations")
            {
                layerToCopy = layer;
                break;
            }
        }

        if(layerToCopy == null)
        {
            return;
        }


        AvatarMaskBodyPart[] allBodyPartsArray = (AvatarMaskBodyPart[])Enum.GetValues(typeof(AvatarMaskBodyPart));
        List<AvatarMaskBodyPart> allBodyParts = new();
        allBodyParts.AddRange(allBodyPartsArray);
        // Remove the invalid body part
        allBodyParts.Remove(AvatarMaskBodyPart.LastBodyPart);

        List<AnimatorControllerLayer> allLayers = new();
        allLayers.AddRange(Controller.layers);

        //allBodyParts.Reverse();

        foreach (AvatarMaskBodyPart bp in allBodyParts)
        {
            if(allLayers.Any(i => i.name == bp.ToString()))
            {
                continue;
            }

            AvatarMask mask = new()
            {
                name = $"{bp}_mask"
            };

            foreach (AvatarMaskBodyPart thisBodyPart in allBodyParts)
            {
                bool isActive = thisBodyPart == bp;
                mask.SetHumanoidBodyPartActive(thisBodyPart, isActive);
            }

            //AnimatorStateMachine stateMachineCopy = Instantiate(layerToCopy.stateMachine);
            AnimatorStateMachine stateMachineCopy = CopyStateMachine(layerToCopy.stateMachine);


            AnimatorControllerLayer layer = new()
            {
                avatarMask = mask,
                blendingMode = layerToCopy.blendingMode,
                defaultWeight = 1,
                name = bp.ToString(),
                stateMachine = stateMachineCopy
            };

            Layers.Add(Controller.layers.Length);
            Controller.AddLayer(layer);
        }
    }


    private void OnApplicationQuit()
    {
        Layers.Reverse();
        foreach (int index in Layers)
        {
            Controller.RemoveLayer(index);
        }
    }



    private static AnimatorStateMachine CopyStateMachine(AnimatorStateMachine toCopy)
    {
        AnimatorStateMachine toReturn = new()
        {
            anyStatePosition = toCopy.anyStatePosition,
            anyStateTransitions = toCopy.anyStateTransitions,
            behaviours = toCopy.behaviours,
            defaultState = toCopy.defaultState,
            entryPosition = toCopy.entryPosition,
            entryTransitions = toCopy.entryTransitions,
            exitPosition = toCopy.exitPosition,
            stateMachines = toCopy.stateMachines,
            states = toCopy.states
        };

        return toReturn;
    }
}
