
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;
public class DrawerInteractable : XRGrabInteractable
{
    public UnityEvent OnDrawerDetach;
    [SerializeField] Transform drawerTransform;
    [SerializeField] XRSocketInteractor keySocket;
    public XRSocketInteractor GetKeySocket => keySocket;
    [SerializeField] XrPhysicsButtonInteractable physicsButton;
    public XrPhysicsButtonInteractable GetPhysicsButton => physicsButton;
    [SerializeField] GameObject keyIndicatorLight;
    [SerializeField] bool isLocked;
    [SerializeField] bool isDetachable;
    [SerializeField] bool isDetached;
    private Transform parentTransform;
    private const string Default_Layer = "Default";
    private const string Grab_Layer = "Grab";
    private bool isGrabbed;
    private Vector3 limitPositions;
    [SerializeField] float drawerLimitZ = 0.8f;
    [SerializeField] private Vector3 limitDistances = new Vector3(.02f, .02f, 0);
    [SerializeField] AudioClip drawerMoveClip;
    public AudioClip GetDrawerMoveClip => drawerMoveClip;
    [SerializeField] AudioClip socketedClip;
    public AudioClip GetSocketedClip => socketedClip;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (keySocket != null)
        {
            keySocket.selectEntered.AddListener(OnDrawerUnlocked);
            keySocket.selectExited.AddListener(OnDrawerLocked);
        }
        parentTransform = transform.parent.transform;
        limitPositions = drawerTransform.localPosition;
        if (physicsButton != null)
        {
            physicsButton.OnBaseEnter.AddListener(OnIsDetachable);
            physicsButton.OnBaseExit.AddListener(OnIsNotDetachable);
        }
    }
    private void OnIsNotDetachable()
    {
        isDetachable = false;
    }
    private void OnIsDetachable()
    {
        isDetachable = true;
    }
    private void OnDrawerLocked(SelectExitEventArgs arg0)
    {
        isLocked = true;
        Debug.Log("****DRAWER LOCKED");
    }
    private void OnDrawerUnlocked(SelectEnterEventArgs arg0)
    {
        isLocked = false;
        if (keyIndicatorLight != null)
        {
            keyIndicatorLight.SetActive(false);
        }
        Debug.Log("****DRAWER UNLOCKED");
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        if (!isLocked)
        {
            transform.SetParent(parentTransform);
            isGrabbed = true;
        }
        else
        {
            ChangeLayerMask(Default_Layer);
        }
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (!isDetached)
        {
            ChangeLayerMask(Grab_Layer);
            isGrabbed = false;
            transform.localPosition = drawerTransform.localPosition;
        }
        else
        {
            rb.isKinematic = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (!isDetached)
        {
            if (isGrabbed && drawerTransform != null)
            {
                drawerTransform.localPosition = new Vector3(drawerTransform.localPosition.x
                , drawerTransform.localPosition.y, transform.localPosition.z);
                CheckLimits();
            }
        }
    }
    private void CheckLimits()
    {
        if (transform.localPosition.x >= limitPositions.x + limitDistances.x ||
            transform.localPosition.x <= limitPositions.x - limitDistances.x)
        {
            ChangeLayerMask(Default_Layer);
        }
        else if (transform.localPosition.y >= limitPositions.y + limitDistances.y ||
            transform.localPosition.y <= limitPositions.y - limitDistances.y)
        {
            ChangeLayerMask(Default_Layer);
        }
        else if (drawerTransform.localPosition.z <= limitPositions.z - limitDistances.z)
        {
            isGrabbed = false;
            drawerTransform.localPosition = limitPositions;
            ChangeLayerMask(Default_Layer);
        }
        else if (drawerTransform.localPosition.z >= drawerLimitZ + limitDistances.z)
        {
            if (!isDetachable)
            {
                isGrabbed = false;
                drawerTransform.localPosition = new Vector3(
                    drawerTransform.localPosition.x,
                    drawerTransform.localPosition.y,
                    drawerLimitZ);
                ChangeLayerMask(Default_Layer);
            }
            else
            {
                DetachDrawer();
            }
        }
    }
    private void DetachDrawer()
    {
        isDetached = true;
        drawerTransform.SetParent(this.transform);
        OnDrawerDetach?.Invoke();
    }
    private void ChangeLayerMask(string mask)
    {
        interactionLayers = InteractionLayerMask.GetMask(mask);
    }
}
