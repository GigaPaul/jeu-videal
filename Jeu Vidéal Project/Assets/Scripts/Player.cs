using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    public CharacterController Controller { get; set; }
    public float MovementSpeed { get; set; }
    public float RotationSpeed { get; set; }
    public Vector3 Velocity { get; set; }
    bool IsRotating { get; set; }
    bool IsQueueing { get; set; }
    private InputSystem InputSystem { get; set; }





    private void Awake()
    {
        // A bouger sûrement, récupère quelle touche est affectée à quelle action dans le menu de réassignation
        InputSystem = new();

        // Load the key bindings
        LoadPlayerKeyBinding();
        LoadUIKeyBinding();
        InitKeyBindingMenu();
        //
    }





    // Start is called before the first frame update
    void Start()
    {
        Controller = GetComponent<CharacterController>();
        MovementSpeed = 20;
        RotationSpeed = 20;
        Velocity = Vector3.zero;

        IsRotating = false;
        IsQueueing = false;

        // If the player is the local player
        if (isLocalPlayer)
        {
            // Deactivate the player model and parent the main camera
            transform.Find("Rotation/Model").gameObject.SetActive(false);
            Camera.main.transform.SetParent(transform.Find("Rotation"), false);
            Camera.main.transform.SetPositionAndRotation(transform.position, transform.rotation);

        }
    }





    // Update is called once per frame
    void Update()
    {

    }





    // FixedUpdate is called every 0.02s (50 calls per second)
    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            // Check if the mouse is hovering a gameObject
            CheckHover();
        }
    }





    // LateUpdate is called after all update functions have been called
    private void LateUpdate()
    {
        if(isLocalPlayer)
        {
            // Rotate and move the player, move the building blueprint
            Rotate();
            Move();
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
        InputSystem.Player.Actions.performed += PerformActions;

        // Alternative actions keys (Left Shift, Left Alt)
        InputSystem.Player.Alts.started += StartAlts;
        InputSystem.Player.Alts.canceled += CancelAlts;

        InputSystem.Player.Menu.performed += OpenMenu;

        InputSystem.UI.Menu.performed += CloseMenu;
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
        IsRotating = true;
    }





    // When the corresponding key is released, the camera stops following the mouse
    private void CancelRotate(InputAction.CallbackContext context)
    {
        IsRotating = false;
    }





    private void Rotate()
    {
        // If the rotation button is held
        if(IsRotating)
        {
            Vector2 Rotation = Mouse.current.delta.ReadValue();
            // Calculate the rotation around the Y axis and apply it
            Vector3 RotationY = Vector3.up * Rotation.x;
            transform.Rotate(RotationSpeed * Time.deltaTime * RotationY);

            // Calculate the rotation around the X axis and apply it
            Vector3 RotationX = Vector3.left * Rotation.y;
            transform.Find("Rotation").Rotate(RotationSpeed * Time.deltaTime * RotationX);
        }
    }





    // 
    private void SetVelocity(InputAction.CallbackContext context)
    {
        Velocity = context.ReadValue<Vector3>();
    }





    private void Move()
    {
        if(Velocity != Vector3.zero)
        {
            Vector3 movement = transform.rotation * Velocity;

            Controller.Move(MovementSpeed * Time.deltaTime * movement);
        }
    }





    private void PerformActions(InputAction.CallbackContext context)
    {
        // Left click
        if(context.ReadValue<float>() == 1)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            //Globals.FocusableMask

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                Pawn hitPawn = hit.transform.GetComponentInParent<Pawn>();
                Globals.FocusedPawn = hitPawn;
            }
            else
            {
                Globals.FocusedPawn = null;
            }
        }
        // Right click
        else
        {
            if (Globals.FocusedPawn != null && Globals.FocusedPawn.IsPlayable())
            {
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Globals.GroundMask))
                {
                    Globals.FocusedPawn.GoTo(hit.point, IsQueueing);
                }
            }
        }
    }





    private void StartAlts(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == -1)
        {
            IsQueueing = true;
        }
    }





    private void CancelAlts(InputAction.CallbackContext context)
    {
        IsQueueing = false;
        //if (context.ReadValue<float>() == -1)
        //{
        //}
    }





    private void CheckHover()
    {
        RaycastHit hover;
        Ray rayHover = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(rayHover, out hover, Mathf.Infinity, Globals.FocusableMask))
        {
            Pawn hoveredPawn = hover.transform.GetComponentInParent<Pawn>();
            Globals.HoveredPawn = hoveredPawn;
        }
        else
        {
            Globals.HoveredPawn = null;
            //Globals.HoveredInteractive = null;
        }
    }
}
