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

        if (isLocalPlayer)
        {
            transform.Find("Rotation/Model").gameObject.SetActive(false);
            Camera.main.transform.SetParent(transform.Find("Rotation"), false);
            Camera.main.transform.position = transform.position;
            Camera.main.transform.rotation = transform.rotation;

        }
        LoadKeyBinding();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (isLocalPlayer)
        {
            CheckHover();
        }
    }


    private void LateUpdate()
    {
        if(isLocalPlayer)
        {
            Rotate();
            Move();
            MoveBlueprint();
        }

    }


    private void LoadKeyBinding()
    {
        InputSystem inputSystem = new InputSystem();
        inputSystem.Player.Enable();

        inputSystem.Player.Move.performed += CheckVelocity;

        inputSystem.Player.Look.started += StartRotate;
        inputSystem.Player.Look.canceled += CancelRotate;

        inputSystem.Player.Actions.performed += PerformActions;

        inputSystem.Player.Alts.started += StartAlts;
        inputSystem.Player.Alts.canceled += CancelAlts;
    }

    private void StartRotate(InputAction.CallbackContext context)
    {
        isRotating = true;
    }

    private void CancelRotate(InputAction.CallbackContext context)
    {
        isRotating = false;
    }

    private void Rotate()
    {
        if(isRotating)
        {
            Vector3 RotationY = Vector3.up * Mouse.current.delta.x.ReadValue();
            transform.Rotate(RotationY * rotationSpeed * Time.deltaTime);

            Vector3 RotationX = Vector3.left * Mouse.current.delta.y.ReadValue();
            transform.Find("Rotation").Rotate(RotationX * rotationSpeed * Time.deltaTime);
        }
    }


    private void Move()
    {
        Vector3 movement = transform.rotation * velocity;

        controller.Move(movement * movementSpeed * Time.deltaTime);
    }

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

    private void CheckVelocity(InputAction.CallbackContext context)
    {
        velocity = context.ReadValue<Vector3>();
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
