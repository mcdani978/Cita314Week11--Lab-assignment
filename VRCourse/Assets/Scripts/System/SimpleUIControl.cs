using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleUIControl : MonoBehaviour
{
    //Serialized Fields
    [Header("Interactables")]
    [SerializeField]
    ButtonInteractable buttonInteractable;

    [SerializeField]
    GameObject keyInteractableLight;

    [Header("String Handling")]

    [SerializeField]
    string[] messageStrings;

    [SerializeField]
    TMP_Text[] messageTexts;

    //Called before first frame update
    void Awake()
    {
        //Tell button interactable to list to method when it is selected
        if (buttonInteractable != null)
        {
            buttonInteractable.selectEntered.AddListener(ButtonInteractablePressed);
        }
    }

    
    
    public void SetText(string message)
    {
        //Update all text elements that should change when button is pressed.
        for (int i = 0; i < messageTexts.Length; i++)
        {
            messageTexts[i].text = message;
        }
    }



    //Method for when button is pressed
    void ButtonInteractablePressed(SelectEnterEventArgs arg0)
    {
        //Temportaty array assignment
        SetText(messageStrings[1]);
        if (keyInteractableLight != null)
        {
            keyInteractableLight.SetActive(true);
        }
    }
}
