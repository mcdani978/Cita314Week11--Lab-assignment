using UnityEngine;
using UnityEngine.Events;

public class DoorInteractable : SimpleHingeInteractable
{
    //Events. UnityEvent is serializable and can be called like a normal method. Works like UI button IsPressed Events.
    public UnityEvent OnOpen;

    //Serialized Fields
    [Header("Door Requirements")]

    [SerializeField]
    Transform doorTransform;

    [SerializeField]
    CombinationLock comboLock;

    [Header("Door Limits")]

    [SerializeField]
    Vector3 rotationLimits;

    [SerializeField]
    Collider closedCollider;

    [SerializeField]
    Collider openCollider;

    [SerializeField]
    Vector3 endRotation;

    //Cashe References
    Vector3 startRotation;

    //Attributes
    float startAngleX;
    bool isClosed;
    bool isOpen;


    protected override void Start()
    {
        base.Start();

        startRotation = transform.localEulerAngles;
        startAngleX = GetAngle(startRotation.x);

        if (comboLock != null)
        {
            comboLock.UnlockAction += UnlockAction_OnUnlocked;
            comboLock.LockAction += LockAction_OnLocked;
        }
    }

    //Inheriting from SimpleHingeInteractable
    protected override void Update()
    {
        base.Update();

        //If door isn't null, Get the door's rotation
        if (doorTransform != null)
        {
            doorTransform.localEulerAngles = new Vector3(
                doorTransform.localEulerAngles.x,
                transform.localEulerAngles.y,
                doorTransform.localEulerAngles.z
                );
        }

        //If the door is selected (OnSelectEntered), check if it's within the door's limits
        if (isSelected)
        {
            CheckLimits();
        }
    }

    //Called when the collider enters a trigger
    void OnTriggerEnter(Collider other)
    {
        //Check if collider is a specific collider (alternative to endless tags)
        if (other == closedCollider)
        {
            isClosed = true;
            ReleaseHinge();
        }
        else if (other == openCollider)
        {
            isOpen = true;
            ReleaseHinge();
        }
    }



    protected override void ResetHinge()
    {
        OnOpen?.Invoke();

        if (isClosed)
        {
            transform.localEulerAngles = startRotation;
        }
        else if (isOpen)
        {
            transform.localEulerAngles = endRotation;
        }
        else
        {
            transform.localEulerAngles = new Vector3(
                startAngleX,
                transform.localEulerAngles.y,
                transform.localEulerAngles.z
                );
        }
    }



    //Method to lock the door hinge when the combo lock is locked
    void LockAction_OnLocked()
    {
        LockHinge();
    }

    //Method to unlock the door hinge when the combo lock is unlocked
    void UnlockAction_OnUnlocked()
    {
        UnlockHinge();
    }

    //Method to check if the door's rotation is within the given limits
    void CheckLimits()
    {
        //Door is not open or closed (it's grabbed)
        isClosed = false;
        isOpen = false;

        //Get the angle of the door between -180 and 180
        float localAngleX = GetAngle(transform.localEulerAngles.x);

        //Release the hinge if the rotation is outside its given limits
        if (localAngleX >= startAngleX + rotationLimits.x || localAngleX <= startAngleX - rotationLimits.x)
        {
            ReleaseHinge();
        }
    }

    //Method to get the coterminal angle between -180 and 180
    float GetAngle(float angle)
    {
        if (angle >= 180)
        {
            angle -= 360;
        }

        return angle;
    }
}
