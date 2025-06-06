using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class XRPhysicsButtonInteractable : XRSimpleInteractable
{
    public UnityEvent OnBaseEnter;
    public UnityEvent OnBaseExit;

    [SerializeField] Collider baseCollider;

    protected override void OhHoverEntered(HoverEnterEventArgs args)
    {
        base.OhHoverEntered(args);
    }

    protected override void OnHoverExited(HoverExitedEventArgs args)
    {
        base.OnHoverExited(args);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(baseCollider != null)
        {
            if(isHovered && other == baseCollider)
            {
                OnBaseEnter?.Invoke();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (baseCollider != null)
        {
            if (other == baseCollider)
            {
                OnBaseExit?.Invoke();   
            }
        }
    }
}
