using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class SimpleHingeInteractable : XRSimpleInteractable
{
    public UnityEvent<SimpleHingeInteractable> OnHingeSelected;

    //Serialized Fields
    [SerializeField]
    Vector3 positionLimits;

    [SerializeField]
    bool isLocked = true;

    [SerializeField]
    AudioClip hingeMoveClip;
    public AudioClip GetHingeMoveClip => hingeMoveClip;

    //Cashe References
    Transform grabHand;
    Collider hingeCollider;
    Vector3 hingePositions;

    //Attributes
    const string DEFAULT_LAYER = "Default";
    const string GRAB_LAYER = "Grab";

    //Called on first fram update
    protected virtual void Start()
    {
        //Get hinge collider
        hingeCollider = GetComponent<Collider>();
    }

    //Virtual b/c of DoorInteractable. Called each frame.
    protected virtual void Update()
    {
        //If the hand is not null (object is grabbed), track it.
        if (grabHand != null)
        {
            TrackHand();
        }
    }

    //Called when player grabs the hinge
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        //If the hinge isn't locked, store reference to the object's transform that grabbed the hinge
        if (!isLocked)
        {
            base.OnSelectEntered(args);
            grabHand = args.interactorObject.transform;
            OnHingeSelected?.Invoke(this);
        }
    }

    //Called when player stops grabbing hinge (manually or automatically)
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        //Stop tracking the hand
        grabHand = null;

        //Allow the object to be grabbed again
        ChangeLayerMask(GRAB_LAYER);

        ResetHinge();
    }



    //Method to unlock the hinge
    public void UnlockHinge()
    {
        isLocked = false;
    }



    //Method for the hinge to look at the player's hand
    void TrackHand()
    {
        //Look at player's hand
        transform.LookAt(grabHand, transform.forward);

        //Store the real-world position of the hinge
        hingePositions = hingeCollider.bounds.center;

        //Check if the player's hand goes too far from the hinge's position. If so, release the hinge
        if (grabHand.position.x >= hingePositions.x + positionLimits.x ||
                   grabHand.position.x <= hingePositions.x - positionLimits.x)
        {
            //Debug.Log("****RELEASE HINGE X");
            ReleaseHinge();
        }
        else if (grabHand.position.y >= hingePositions.y + positionLimits.y ||
            grabHand.position.y <= hingePositions.y - positionLimits.y)
        {
            //Debug.Log("****RELEASE HINGE Y");
            ReleaseHinge();
        }
        else if (grabHand.position.z >= hingePositions.z + positionLimits.z ||
            grabHand.position.z <= hingePositions.z - positionLimits.z)
        {
            //Debug.Log("****RELEASE HINGE Z");
            ReleaseHinge();
        }
    }

    //Abstract method that will be used to reset the hinge's position
    protected abstract void ResetHinge();

    //Method to change the interaction layer mask of the hinge for grabbing/releasing the hinge
    void ChangeLayerMask(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }

    //Method to stop player from grabbing the hinge
    public void ReleaseHinge()
    {
        ChangeLayerMask(DEFAULT_LAYER);
    }

    //Method to lock the hinge
    public void LockHinge()
    {
        isLocked = true;
    }
}
