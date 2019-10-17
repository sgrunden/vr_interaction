using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class CustomHand : MonoBehaviour
{
    public SteamVR_Action_Boolean triggerAction = null;
    public SteamVR_Action_Boolean touchpadAction = null;

    private SteamVR_Behaviour_Pose pose = null;
    private FixedJoint joint = null;

    private CustomInteractable currentInteractable = null;
    public List<CustomInteractable> ContactInteractables = new List<CustomInteractable>();

    private Vector3 oldPosition;
    private Quaternion oldRotation;

    private CustomHaptics haptics;


    [HideInInspector]
    public bool isPickingUp = false;

    private void Awake()
    {
        pose = GetComponent<SteamVR_Behaviour_Pose>();
        joint = GetComponent<FixedJoint>();
        haptics = GetComponent<CustomHaptics>();
    }

    private void Update()
    {
        //Trigger Down = Pickup Object
        if (triggerAction.GetStateDown(pose.inputSource))
        {
            print(pose.inputSource + "Trigger Down");

            if(currentInteractable != null)
            {
                currentInteractable.Action();
                return;
            }
            haptics.Pulse(1, 150, 75, SteamVR_Input_Sources.LeftHand);
            Pickup();
        }

        //Touchpad Up = Drop Object
        if (touchpadAction.GetStateDown(pose.inputSource))
        {
            print(pose.inputSource + "Touchpad Up");
            Drop();
        }

    }

    void OnTriggerEnter(Collider collider)
    {
        if (!collider.gameObject.CompareTag("Interactable"))
        {
            return;
        }

        ContactInteractables.Add(collider.gameObject.GetComponent<CustomInteractable>()); //Adds net to list of interactables that are nearby
    }

    void OnTriggerExit(Collider collider)
    {
        if (!collider.gameObject.CompareTag("Interactable"))
        {
            return;
        }

        ContactInteractables.Remove(collider.gameObject.GetComponent<CustomInteractable>()); //Removes net from list of interactables that are nearby
    }

    public void Pickup()
    {
        //Get Nearest
        currentInteractable = GetNearestInteractable();
        oldPosition = currentInteractable.transform.position;
        oldRotation = currentInteractable.transform.rotation;

        //Null check - do we have something?
        if (!currentInteractable)
        {
            return;
        }

        //Already held? Check
        if(currentInteractable.activeHand)
        if(currentInteractable.activeHand)
        {
            currentInteractable.activeHand.Drop();
        }

        //Position to controller
        currentInteractable.ApplyOffset(transform);
       
        //Attach
        Rigidbody targetBody = currentInteractable.GetComponent<Rigidbody>();
        joint.connectedBody = targetBody;


        //Set Active Hand
        currentInteractable.activeHand = this;

    }

    public void Drop()
    {
        //Null Check
        if (!currentInteractable)
        {
            return;
        }

        //Detach from joint
        joint.connectedBody = null;

        //Return to original position
        currentInteractable.transform.position = oldPosition;
        currentInteractable.transform.rotation = oldRotation;

        //Clear activeHand/currentInteractable
        currentInteractable.activeHand = null;
        currentInteractable = null;
    }

    private CustomInteractable GetNearestInteractable()
    {
        CustomInteractable nearest = null; 
        float minDistance = float.MaxValue;
        float distance = 0.0f;

        foreach(CustomInteractable interactable in ContactInteractables)
        {
            distance = (interactable.transform.position - transform.position).sqrMagnitude;

            if(distance < minDistance)
            {
                minDistance = distance;
                nearest = interactable;
            }
        }
        return nearest;
    }


}
