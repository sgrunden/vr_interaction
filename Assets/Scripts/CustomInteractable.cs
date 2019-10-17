using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

[RequireComponent(typeof(Rigidbody))]
public class CustomInteractable : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 0, 0.1f);
    private Quaternion netRotation = Quaternion.Euler(85, 0, 0);

    private int butterfliesCaught = 0;
    private int distractorsHit = 0;

    private CustomHaptics haptics;

    [HideInInspector]
    public CustomHand activeHand;

    public void Awake()
    {
        haptics = GetComponent<CustomHaptics>();
    }

    public virtual void Action() //Virtual so that any scripts that inherit from interactable can override this section.
    {
        print("Action");
    }

    public void ApplyOffset(Transform hand) //Sets parent of interactable object to the controller, zero outs its rotation, applies offset, then unparents it.
    {
        transform.SetParent(hand);
        transform.localRotation = netRotation; //Zeros it out, can set to any angle here with specific variable.
        transform.localPosition = offset;
        transform.SetParent(null);
    }

    private void OnCollisionEnter(Collision col) //Net will destroy any gameobjects tagged as Butterfly on collision and add to the total number of butterflies caught.
    {
        if (col.gameObject.CompareTag("Butterfly"))
        {
            Destroy(col.gameObject);
            butterfliesCaught += 1;
            Debug.Log("Number of Butterflies Caught: " + butterfliesCaught);
        }
        else if(col.gameObject.CompareTag("Distractor")) //Keeps track of any distractors that were hit by the net as well.
        {
            haptics.Pulse(1, 130, 75, SteamVR_Input_Sources.LeftHand);
            Debug.Log("That's a distractor, try again!");
            distractorsHit += 1; 
        }
    }
}
