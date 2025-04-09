using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonInteractable : XRSimpleInteractable
{
    //Serialized Fields
    [SerializeField]
    Color[] buttonColors = new Color[4];

    [SerializeField]
    Image buttonImage;

    //Cashe References
    Color buttonNormalColor;
    Color buttonHighlightedColor;
    Color buttonPressedColor;
    Color buttonSelectedColor;

    //Attributes
    bool isPressed;

    protected override void Awake()
    {
        base.Awake();

        //Set the colors of the different button states
        buttonNormalColor = buttonColors[0];
        buttonHighlightedColor = buttonColors[1];
        buttonPressedColor = buttonColors[2];
        buttonSelectedColor = buttonColors[3];

        //Set image color to normal color
        buttonImage.color = buttonNormalColor;

    }

    //Controller hovered over button
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        isPressed = false;

        buttonImage.color = buttonHighlightedColor;
    }

    //Controller stopped hovering over button
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);

        if (isPressed) { return; }
        buttonImage.color = buttonNormalColor;
    }

    //Button has been selected
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (isPressed) { return; }
        isPressed = true;
        buttonImage.color = buttonPressedColor;
    }
    
    //Button is no longer selected
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        buttonImage.color = buttonSelectedColor;
    }

    //Public method to set color to its normal color
    public void SetColorToNormal()
    {
        SetButtonColor(buttonNormalColor);
    }

    //Public method to set color to given color
    public void SetButtonColor(Color newColor)
    {
        buttonImage.color = newColor;
    }
}
