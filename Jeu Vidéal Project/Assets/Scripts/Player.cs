using Mirror;
using System.Collections;
using System.Collections.Generic;
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

        // Load the key bindings
        LoadKeyBinding();
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
            MoveBlueprint();
        }

    }





    // Association of keybindings and their corresponding methods
    private void LoadKeyBinding()
    {
        // Initiate the input system and enable the Player action map
        InputSystem inputSystem = new InputSystem();
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





    // To redo entirely to be honest
    private void MoveBlueprint()
    {
        if(Globals.BuildingBlueprint != null)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, Globals.GroundMask))
            {
                //// Temp
                //if (Globals.BuildingBlueprint == null)
                //{
                //    GameObject shopPrefab = Resources.Load("Prefabs/SM_Bld_Shop_Corner_01") as GameObject;
                //    GameObject shopGo = Instantiate(shopPrefab, hit.point, Quaternion.identity);
                //    Building shop = shopGo.GetComponent<Building>();
                //    shop.ToBlueprint();
                //}
                ////

                Vector3 pos = new Vector3(
                    hit.point.x - Globals.BuildingBlueprint.rb.centerOfMass.x,
                    hit.point.y,
                    hit.point.z - Globals.BuildingBlueprint.rb.centerOfMass.z);
                Globals.BuildingBlueprint.transform.position = pos;
            }
        }
    }





    private void PerformActions(InputAction.CallbackContext context)
    {
        // Left click
        if(context.ReadValue<float>() == 1)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, Globals.FocusableMask))
                {
                    Neighbour HittedNeighbour = hit.transform.GetComponentInParent<Neighbour>();
                    Globals.FocusedNeighbour = HittedNeighbour;
                }
                else
                {
                    Globals.FocusedNeighbour = null;

                    if (Globals.BuildingBlueprint != null)
                    {
                        Building newBuilding = Instantiate(Globals.BuildingBlueprint, Vector3.zero, Quaternion.identity);
                        newBuilding.Place(Globals.BuildingBlueprint.transform);
                    }
                }
            }
        }
        // Right click
        else
        {
            if (Globals.FocusedNeighbour != null)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, Globals.GroundMask))
                {
                    // If the player isn't queueing waypoints, empty the patrol
                    if (!isQueueing)
                    {
                        Globals.FocusedNeighbour.EmptyPatrol();
                    }

                    // Add waypoint to patrol
                    Globals.FocusedNeighbour.AddToPatrol(hit.point);
                }
            }
        }
    }





    private void StartAlts(InputAction.CallbackContext context)
    {
        if(context.ReadValue<float>() == -1)
        {
            isQueueing = true;
        }
    }





    private void CancelAlts(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() == -1)
        {
            isQueueing = false;
        }
    }





    private void CheckHover()
    {
        RaycastHit hover;
        Ray rayHover = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(rayHover, out hover, Mathf.Infinity, Globals.FocusableMask))
        {
            Globals.HoveredNeighbour = hover.transform.GetComponentInParent<Neighbour>();
            //Globals.HoveredInteractive = hover.transform.GetComponentInParent<InteractiveObject>();
            //GameObject.Find("Neighbour").GetComponent<Neighbour>().AddToPatrol(hit.point);
        }
        else
        {
            Globals.HoveredNeighbour = null;
            //Globals.HoveredInteractive = null;
        }
    }
}
