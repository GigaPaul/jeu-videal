using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private InputSystem InputSystem { get; set; }
    public Player Player;
    bool LeftClickIsHeld = false;
    bool RightClickIsHeld = false;
    public RectTransform SelectionArea;

    #nullable enable
    Pawn? FocusedObject { get; set; }
    #nullable disable

    private void Awake()
    {
        InputSystem = new();

        // Load the key bindings
        LoadPlayerKeyBinding();
        LoadUIKeyBinding();
        InitKeyBindingMenu();
        //
    }




    private void FixedUpdate()
    {
        if(LeftClickIsHeld)
        {
            GenerateSelection();
        }
    }





    // Association of keybindings and their corresponding methods
    private void LoadPlayerKeyBinding()
    {
        // Initiate the input system and enable the Player action map
        InputSystem.Player.Enable();

        // Movement keys (WASD, space, ctrl)
        InputSystem.Player.Move.performed += SetVelocity;

        // Camera rotation keys (Middle mouse)
        InputSystem.Player.Look.started += StartRotate;
        InputSystem.Player.Look.canceled += CancelRotate;

        // Actions keys (Left mouse, right mouse)
        InputSystem.Player.Actions.started += StartActions;
        InputSystem.Player.Actions.performed += PerformActions;
        InputSystem.Player.Actions.canceled += CancelActions;

        // Alternative actions keys (Left Shift, Left Alt)
        InputSystem.Player.Alts.started += StartAlts;
        InputSystem.Player.Alts.canceled += CancelAlts;

        InputSystem.Player.Menu.performed += OpenMenu;

        InputSystem.UI.Menu.performed += CloseMenu;
    }





    // 
    private void SetVelocity(InputAction.CallbackContext context)
    {
        Player.Velocity = context.ReadValue<Vector3>();
    }





    private void StartActions(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<float>();

        switch (value)
        {
            // Left click
            case 1:
                LeftClickIsHeld = true;
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Pawn hitPawn = null;

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Globals.FocusableMask))
                {
                    hitPawn = hit.transform.GetComponentInParent<Pawn>();
                    FocusedObject = hitPawn;
                }

                if (hitPawn == null)
                {
                    SelectionArea.position = Mouse.current.position.ReadValue();
                    SelectionArea.gameObject.SetActive(true);
                    GenerateSelection();
                }
                break;



            // Right click
            case -1:
                RightClickIsHeld = true;
                break;
        }
    }





    private void PerformActions(InputAction.CallbackContext context)
    {

    }





    private void CancelActions(InputAction.CallbackContext context)
    {
        if(LeftClickIsHeld)
        {
            Globals.FocusedPawn = FocusedObject;
            LeftClickIsHeld = false;
            SelectionArea.gameObject.SetActive(false);
        }



        if(RightClickIsHeld)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit rightClickHit;

            // Right click on focusable object
            if (Physics.Raycast(ray, out rightClickHit, Mathf.Infinity, Globals.FocusableMask))
            {
                if(rightClickHit.transform.GetComponentInParent<Pawn>())
                {
                    if(Globals.FocusedPawn != null)
                    {
                        Pawn interactedPawn = rightClickHit.transform.GetComponentInParent<Pawn>();

                        if(interactedPawn.Faction.Id == "g_bandits")
                        {
                            Globals.FocusedPawn._PawnCombat.ForceTarget(interactedPawn);
                        }
                    }

                }
            }
            // Right click on ground
            else if (Physics.Raycast(ray, out rightClickHit, Mathf.Infinity, Globals.GroundMask))
            {
                if (Globals.FocusedPawn != null && Globals.FocusedPawn.IsPlayable())
                {
                    Globals.FocusedPawn.GoTo(rightClickHit.point, Player.IsQueueing);
                }
            }

            RightClickIsHeld = false;
        }
    }





    private void StartAlts(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == -1)
        {
            Player.IsQueueing = true;
        }
    }





    private void CancelAlts(InputAction.CallbackContext context)
    {
        Player.IsQueueing = false;
    }





    private void LoadUIKeyBinding()
    {

    }





    private void InitKeyBindingMenu()
    {
        Globals.PauseMenu = GameObject.Find("Menu");
        Transform KBForward = Globals.PauseMenu.transform.GetComponentsInChildren<Transform>().FirstOrDefault(i => i.name == "KeybindForward");
        TextMeshProUGUI KBForwardBTN = KBForward.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(i => i.name == "TextButton");
        string forwardKeyPath = InputSystem.Player.Move.bindings[5].path;
        string forwardKeyString = InputControlPath.ToHumanReadableString(forwardKeyPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        KBForwardBTN.text = forwardKeyString;

        Globals.PauseMenu.SetActive(false);
    }





    private void OpenMenu(InputAction.CallbackContext context)
    {
        InputSystem.Player.Disable();
        InputSystem.UI.Enable();
        Globals.PauseMenu.SetActive(true);
    }




    private void CloseMenu(InputAction.CallbackContext context)
    {
        InputSystem.Player.Enable();
        InputSystem.UI.Disable();
        Globals.PauseMenu.SetActive(false);
    }







    // When the corresponding key is held, the camera starts following the mouse
    private void StartRotate(InputAction.CallbackContext context)
    {
        Player.IsRotating = true;
    }





    // When the corresponding key is released, the camera stops following the mouse
    private void CancelRotate(InputAction.CallbackContext context)
    {
        Player.IsRotating = false;
    }




    private void GenerateSelection()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 selectionPos = new(SelectionArea.transform.position.x, SelectionArea.transform.position.y);
        Vector2 size = mousePosition - selectionPos;
        SelectionArea.sizeDelta = size;
    }
}
