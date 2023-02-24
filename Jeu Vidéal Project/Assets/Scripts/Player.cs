using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
{
    public CharacterController Controller;
    public GameObject Model;
    public Transform RotationAxis;
    public float MovementSpeed { get; set; }
    public float RotationSpeed { get; set; }
    public Vector3 Velocity { get; set; }
    public bool IsRotating { get; set; }
    public bool IsQueueing { get; set; }





    // Start is called before the first frame update
    void Start()
    {
        MovementSpeed = 20;
        RotationSpeed = 20;
        Velocity = Vector3.zero;

        IsRotating = false;
        IsQueueing = false;

        // If the player is the local player
        if (isLocalPlayer)
        {
            InputManager inputManager = FindObjectOfType<InputManager>();
            inputManager.Player = this;

            // Deactivate the player model and parent the main camera
            Model.SetActive(false);
            Camera.main.transform.SetParent(RotationAxis, false);
            Camera.main.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }
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
            RotationAxis.Rotate(RotationSpeed * Time.deltaTime * RotationX);
        }
    }





    private void Move()
    {
        if(Velocity != Vector3.zero)
        {
            Vector3 movement = transform.rotation * Velocity;

            Controller.Move(MovementSpeed * Time.deltaTime * movement);
        }
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
