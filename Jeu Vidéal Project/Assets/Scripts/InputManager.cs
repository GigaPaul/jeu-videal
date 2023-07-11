using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    private InputSystem InputSystem { get; set; }
    public Player Player;
    bool LeftClickIsHeld = false;
    bool RightClickIsHeld = false;
    public RectTransform SelectionArea;
    public bool CursorOverUI = false;

    #nullable enable
    Interactive? FocusedObject { get; set; }
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
        CursorOverUI = EventSystem.current.IsPointerOverGameObject();
        if (LeftClickIsHeld)
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
        // Check if the cursor isn't over a button or something
        if (CursorOverUI)
        {
            return;
        }




        float value = context.ReadValue<float>();

        switch (value)
        {
            // Left click
            case 1:
                LeftClickIsHeld = true;
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                Interactive hitInteractive = null;
                SelectionArea.position = Mouse.current.position.ReadValue();


                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Globals.FocusableMask))
                {
                    hitInteractive = hit.transform.GetComponentInParent<Interactive>();
                    FocusedObject = hitInteractive;
                }

                if (hitInteractive == null)
                {
                    //SelectionArea.gameObject.SetActive(true);
                    //GenerateSelection();
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
        // Check if the cursor isn't over a button or something
        if (CursorOverUI)
        {
            return;
        }
    }





    private void CancelActions(InputAction.CallbackContext context)
    {
        CancelLeftClickActions();
        CancelRightClickActions();
    }





    private void CancelLeftClickActions()
    {
        if (!LeftClickIsHeld)
        {
            return;
        }

        if(!CursorOverUI)
        {
            if(Globals.FocusedInteractive != null && Globals.FocusedInteractive != FocusedObject)
            {
                Globals.FocusedInteractive.Unfocus();
            }


            if (FocusedObject != null)
            {
                FocusedObject.Focus();
            }
        }

        LeftClickIsHeld = false;
        SelectionArea.gameObject.SetActive(false);
        FocusedObject = null;
    }





    private void CancelRightClickActions()
    {
        if (!RightClickIsHeld)
        {
            return;
        }


        // So it executes only once
        RightClickIsHeld = false;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        //RaycastHit rightClickHit;

        // Right click on focusable object
        if (Physics.Raycast(ray, out RaycastHit focusableHit, Mathf.Infinity, Globals.FocusableMask))
        {
            // The right click needs to be on a Pawn
            if (focusableHit.transform.GetComponentInParent<Pawn>() == null)
            {
                return;
            }

            // We need an Interactive to be focused to perform actions
            if (Globals.FocusedInteractive == null)
            {
                return;
            }


            // If the focused interactive is a pawn and can attack the right clicked pawn, make them attack
            if (Globals.FocusedInteractive.IsPawn(out Pawn focusedPawn))
            {
                Pawn interactedPawn = focusableHit.transform.GetComponentInParent<Pawn>();

                if (!focusedPawn.CanAttack(interactedPawn))
                {
                    return;
                }

                focusedPawn._PawnCombat.ForceTarget(interactedPawn);
            }
        }
        // Right click on ground
        else if (Physics.Raycast(ray, out RaycastHit groundHit, Mathf.Infinity, Globals.GroundMask))
        {
            // We need a interactive to be focused for it to move
            if (Globals.FocusedInteractive == null)
            {
                return;
            }

            // If the interactive is a pawn and playable, order them to move
            if (Globals.FocusedInteractive.IsPawn(out Pawn focusedPawn))
            {
                if (!focusedPawn.IsPlayable())
                {
                    return;
                }

                // Untargets the current target if there is one
                if (focusedPawn._PawnCombat.CurrentTarget != null || focusedPawn._PawnCombat.ForcedTarget != null)
                {
                    focusedPawn._PawnCombat.ForceTarget(null);
                }

                // Go to the location
                focusedPawn.GoTo(groundHit.point, Player.IsQueueing);
            }
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

        float distance = Vector2.Distance(mousePosition, selectionPos);

        if(distance < 15)
        {
            return;
        }

        if(!SelectionArea.gameObject.activeSelf)
        {
            SelectionArea.gameObject.SetActive(true);
        }

        Vector2 size = mousePosition - selectionPos;
        SelectionArea.sizeDelta = size;
    }
}
