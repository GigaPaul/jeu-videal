using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbour : NetworkBehaviour
{
    public CharacterController controller { get; set; }
    public List<Vector3> patrol { get; set; }
    Vector3? positionTarget { get; set; }
    Vector3 rotationTarget { get; set; }
    public float horizontalDirection { get; set; }
    public float verticalDirection { get; set; }
    public float maxSpeed { get; set; }
    public float rotationSpeed { get; set; }
    public Transform moodSphere { get; set; }

    public GameObject hoverRing { get; set; }
    public GameObject focusRing { get; set; }



    // Start is called before the first frame update
    void Start()
    {
        horizontalDirection = 0f;
        verticalDirection = 0f;
        maxSpeed = 5;
        rotationSpeed = 10;
        controller = GetComponent<CharacterController>();
        patrol = new();
        moodSphere = transform.Find("Mood");


        GameObject stateRingsPrefab = Resources.Load("Prefabs/Rings") as GameObject;
        GameObject stateRings = Instantiate(stateRingsPrefab, transform.position + Vector3.up * 0.15f, Quaternion.identity, transform);
        hoverRing = stateRings.transform.Find("HoverRing").gameObject;
        focusRing = stateRings.transform.Find("FocusRing").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        Rotate();
        Move();
    }

    void FixedUpdate()
    {
        ManageRings();
    }

    private void Rotate()
    {
        if (rotationTarget != Vector3.zero)
        {
            Quaternion AbsoluteRotation = Quaternion.LookRotation(rotationTarget);
            Quaternion NewRotation = Quaternion.Euler(0, AbsoluteRotation.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, NewRotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void ManageRings()
    {
        // Focus
        if(IsFocused() && !focusRing.activeInHierarchy)
        {
            focusRing.SetActive(true);
        }
        else if(!IsFocused() && focusRing.activeInHierarchy)
        {
            focusRing.SetActive(false);
        }

        // Hover
        if (IsHovered() && !hoverRing.activeInHierarchy)
        {
            hoverRing.SetActive(true);
        }
        else if (!IsHovered() && hoverRing.activeInHierarchy)
        {
            hoverRing.SetActive(false);
        }
    }

    private void Move()
    {
        if(patrol.Count > 0)
        {
            if(positionTarget != null)
            {
                if (verticalDirection != 1)
                    verticalDirection = 1;

                Face((Vector3)positionTarget);

                Vector3 RelativePos = (Vector3)positionTarget - transform.position;
                Quaternion TargetedRotation = Quaternion.LookRotation(RelativePos);
                Vector3 RotationOffset = TargetedRotation.eulerAngles - transform.rotation.eulerAngles;
                Vector3 Direction = controller.transform.forward * verticalDirection + controller.transform.right * horizontalDirection;
                Vector3 Movement = Quaternion.Euler(0, RotationOffset.y, 0) * Direction;
                controller.Move(maxSpeed * Time.deltaTime * Movement);

                if (((Vector3)positionTarget - transform.position).sqrMagnitude < 0.1)
                {
                    if (verticalDirection != 0)
                        verticalDirection = 0;

                    positionTarget = null;
                    patrol.RemoveAt(0);
                }

                // Keep the character on ground level
                float newY = Terrain.activeTerrain.SampleHeight(transform.position);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
            else
            {
                GoTo(patrol[0]);
            }
        }
    }



    public bool IsFocused()
    {
        return Globals.FocusedNeighbour == this;
    }


    public bool IsHovered()
    {
        return Globals.HoveredNeighbour == this;
    }



    protected virtual void GoTo(Vector3 target)
    {
        positionTarget = target;
    }



    public void Face(Vector3 position)
    {
        Vector3 relativeTarget = (position - transform.position).normalized;
        rotationTarget = relativeTarget;
    }



    [Command(requiresAuthority = false)]
    public void AddToPatrol(Vector3 position)
    {
        RpcHandleAddToPatrol(position);
    }

    [Command(requiresAuthority = false)]
    public void EmptyPatrol()
    {
        RpcHandleEmptyPatrol();
    }



    [ClientRpc]
    private void RpcHandleAddToPatrol(Vector3 position)
    {
        patrol.Add(position);
    }



    [ClientRpc]
    private void RpcHandleEmptyPatrol()
    {
        patrol.Clear();
        positionTarget = null;
    }
}
