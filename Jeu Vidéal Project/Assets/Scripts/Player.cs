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
    public CharacterController controller { get; set; }
    public float movementSpeed { get; set; }
    public float rotationSpeed { get; set; }
    public Vector3 velocity { get; set; }
    bool isRotating { get; set; }
    bool isQueueing { get; set; }
    private InputSystem inputSystem { get; set; }





    private void Awake()
    {
        // A bouger sûrement, récupère quelle touche est affectée à quelle action dans le menu de réassignation
        inputSystem = new();

        // Load the key bindings
        LoadPlayerKeyBinding();
        LoadUIKeyBinding();
        InitKeyBindingMenu();
        //
    }





    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        movementSpeed = 20;
        rotationSpeed = 20;
        velocity = Vector3.zero;

        isRotating = false;
        isQueueing = false;

        // If the player is the local player
        if (isLocalPlayer)
        {
            // Deactivate the player model and parent the main camera
            transform.Find("Rotation/Model").gameObject.SetActive(false);
            Camera.main.transform.SetParent(transform.Find("Rotation"), false);
            Camera.main.transform.position = transform.position;
            Camera.main.transform.rotation = transform.rotation;

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
        inputSystem.Player.Enable();

        // Movement keys (WASD, space, ctrl)
        inputSystem.Player.Move.performed += SetVelocity;

        // Camera rotation keys (Middle mouse)
        inputSystem.Player.Look.started += StartRotate;
        inputSystem.Player.Look.canceled += CancelRotate;

        // Actions keys (Left mouse, right mouse)
        inputSystem.Player.Actions.performed += PerformActions;

        // Alternative actions keys (Left Shift, Left Alt)
        inputSystem.Player.Alts.started += StartAlts;
        inputSystem.Player.Alts.canceled += CancelAlts;

        inputSystem.Player.Menu.performed += OpenMenu;

        inputSystem.UI.Menu.performed += CloseMenu;
    }





    private void LoadUIKeyBinding()
    {

    }





    private void InitKeyBindingMenu()
    {
        Globals.PauseMenu = GameObject.Find("Menu");
        Transform KBForward = Globals.PauseMenu.transform.GetComponentsInChildren<Transform>().FirstOrDefault(i => i.name == "KeybindForward");
        TextMeshProUGUI KBForwardBTN = KBForward.GetComponentsInChildren<TextMeshProUGUI>().FirstOrDefault(i => i.name == "TextButton");
        string forwardKeyPath = inputSystem.Player.Move.bindings[5].path;
        string forwardKeyString = InputControlPath.ToHumanReadableString(forwardKeyPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
        KBForwardBTN.text = forwardKeyString;

        Globals.PauseMenu.SetActive(false);
    }





    private void OpenMenu(InputAction.CallbackContext context)
    {
        inputSystem.Player.Disable();
        inputSystem.UI.Enable();
        Globals.PauseMenu.SetActive(true);
    }




    private void CloseMenu(InputAction.CallbackContext context)
    {
        inputSystem.Player.Enable();
        inputSystem.UI.Disable();
        Globals.PauseMenu.SetActive(false);
    }







    // When the corresponding key is held, the camera starts following the mouse
    private void StartRotate(InputAction.CallbackContext context)
    {
        isRotating = true;
    }





    // When the corresponding key is released, the camera stops following the mouse
    private void CancelRotate(InputAction.CallbackContext context)
    {
        isRotating = false;
    }





    private void Rotate()
    {
        // If the rotation button is held
        if(isRotating)
        {
            Vector2 Rotation = Mouse.current.delta.ReadValue();
            // Calculate the rotation around the Y axis and apply it
            Vector3 RotationY = Vector3.up * Rotation.x;
            transform.Rotate(RotationY * rotationSpeed * Time.deltaTime);

            // Calculate the rotation around the X axis and apply it
            Vector3 RotationX = Vector3.left * Rotation.y;
            transform.Find("Rotation").Rotate(RotationX * rotationSpeed * Time.deltaTime);
        }
    }





    // 
    private void SetVelocity(InputAction.CallbackContext context)
    {
        velocity = context.ReadValue<Vector3>();
    }





    private void Move()
    {
        if(velocity != Vector3.zero)
        {
            Vector3 movement = transform.rotation * velocity;

            controller.Move(movement * movementSpeed * Time.deltaTime);
        }
    }





    private void PerformActions(InputAction.CallbackContext context)
    {
        // Left click
        if(context.ReadValue<float>() == 1)
        {
            //if (!EventSystem.current.IsPointerOverGameObject())
            //{
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, Globals.FocusableMask))
                {
                    Pawn hitPawn = hit.transform.GetComponentInParent<Pawn>();
                    Globals.FocusedPawn = hitPawn;
                }
                else
                {
                    Globals.FocusedPawn = null;
                }
            //}
        }
        // Right click
        else
        {
            if (Globals.FocusedPawn != null && Globals.FocusedPawn.IsPlayable())
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, Globals.GroundMask))
                {
                    // If the player is queueing waypoints
                    Globals.FocusedPawn.GoTo(hit.point, isQueueing);
                    //if (isQueueing)
                    //{
                    //    Globals.FocusedPawn.patrol.Add(hit.point);
                    //}
                    //else
                    //{
                    //    Globals.FocusedPawn.patrol.Clear();
                    //    Globals.FocusedPawn.GoTo(hit.point);
                    //}
                }
            }
        }
    }





    private void StartAlts(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == -1)
        {
            isQueueing = true;
        }
    }





    private void CancelAlts(InputAction.CallbackContext context)
    {
        isQueueing = false;
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
