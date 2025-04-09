using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DrawerInteractable : XRGrabInteractable
{
    //Serialized Fields

    [Header("Game Objects")]
    
    [SerializeField]
    XRSocketInteractor socketInteractor;
    public XRSocketInteractor GetSocketIntractor => socketInteractor;

    [SerializeField]
    Transform drawerTransform;

    [Header("Object Properties")]
    
    [SerializeField]
    Vector3 limitDistances = new Vector3(.01f, .01f, 0);

    [SerializeField]
    bool isLocked = true;

    [SerializeField]
    float maxZPos = 0.9f;

    [SerializeField]
    AudioClip drawerMoveClip;

    public AudioClip GetMoveClip => drawerMoveClip;

    [SerializeField]
    AudioClip socketedClip;
    public AudioClip GetSocketedClip => socketedClip;

    //Cashe References
    Transform parentTransform;
    Vector3 limitPositions;

    //Attributes
    const string DEFAULT_LAYER = "Default";
    const string GRAB_LAYER = "Grab";
    bool isGrabbed = false;

    //XRSocketInteractable HAS AN AWAKE METHOD!!! DO NOT OVERRIDE ITS AWAKE METHOD WITHOUT CALLING IT
    protected override void Awake()
    {
        //Call XRGrabInteractable awake method
        base.Awake();

        //If the socket interactor isn't null, tell it to listen to methods when it is select and unselected
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.AddListener(OnDrawerUnlocked);
            socketInteractor.selectExited.AddListener(OnDrawerLocked);
        }

        //Get parent object's transform
        parentTransform = transform.parent.transform;

        //Set the limit position
        limitPositions = drawerTransform.localPosition;
    }

    void Update()
    {
        if (isGrabbed && drawerTransform != null)
        {
            drawerTransform.localPosition = new Vector3(
                drawerTransform.localPosition.x,
                drawerTransform.localPosition.y,
                transform.localPosition.z);

            CheckPositionLimits();
        }
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (isLocked)
        {
            //Change interaction layer mask
            ChangeLayerMask(DEFAULT_LAYER);
        }
        else
        {
            //Prevent the child object from disconneceting from the parent when grabbed
            //This will allow us to track its local z position
            transform.SetParent(parentTransform);
            isGrabbed = true;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);

        //Allow the player to grab the object again
        ChangeLayerMask(GRAB_LAYER);

        //Object is not grabbed
        isGrabbed = false;

        //Reset the object's local position
        transform.localPosition = drawerTransform.localPosition;
    }



    //Method to lock the drawer
    void OnDrawerLocked(SelectExitEventArgs arg0)
    {
        isLocked = true;
        Debug.Log(isLocked);
    }

    //Method to unlock the drawer
    void OnDrawerUnlocked(SelectEnterEventArgs arg0)
    {
        isLocked = false;
        Debug.Log(isLocked);
    }

    //Method to check if the drawer is within the given positional limits
    void CheckPositionLimits()
    {
        //If position is outside limits, set layer mask to default to stop player from grabbing the object
        if (transform.localPosition.x >= limitPositions.x + limitDistances.x ||
            transform.localPosition.x <= limitPositions.x - limitDistances.x)
        {
            ChangeLayerMask(DEFAULT_LAYER);
        }
        else if (transform.localPosition.y >= limitPositions.y + limitDistances.y ||
            transform.localPosition.y <=  limitPositions.y - limitDistances.y)
        {
            ChangeLayerMask(DEFAULT_LAYER);
        }
        else if (drawerTransform.localPosition.z <= limitPositions.z - limitDistances.z)
        {
            isGrabbed = false;
            ChangeLayerMask(DEFAULT_LAYER);
            drawerTransform.localPosition = limitPositions;
        }
        else if (drawerTransform.localPosition.z >= maxZPos + limitDistances.z)
        {
            isGrabbed = false;
            drawerTransform.localPosition = new Vector3(
                drawerTransform.localPosition.x,
                drawerTransform.localPosition.y,
                maxZPos);
            ChangeLayerMask(DEFAULT_LAYER);
        }
    }

    //Method to change the object's interaction layer mask for grabbing/ungrabbing purposes
    void ChangeLayerMask(string layerMask)
    {
        interactionLayers = InteractionLayerMask.GetMask(layerMask);
    }
}
