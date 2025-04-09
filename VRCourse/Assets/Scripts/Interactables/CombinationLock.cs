using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class CombinationLock : MonoBehaviour
{
    //Actions. Unlike UnityEvent, UnityAction is more specific to scripting purposes only
    public UnityAction UnlockAction;
    public UnityAction ComboButtonPressed;
    void OnComboButtonPress() => ComboButtonPressed?.Invoke();
    void OnUnlocked() => UnlockAction?.Invoke();
    /*
     void OnUnlocked(){
        UnlockAction?.Invoke();
    }
     */

    public UnityAction LockAction;
    void OnLocked() => LockAction?.Invoke();
    /*
     void OnLocked(){
        LockAction?.Invoke();
    }
     */


    //SerializedFields
    [Header("Combo Lock Properties")]

    [SerializeField]
    string numberCombination = "0412";

    [SerializeField]
    ButtonInteractable[] comboButtons = new ButtonInteractable[4];

    [SerializeField]
    TMP_Text textInput;

    [SerializeField]
    bool isLocked = true;


    [Header("Audio")]

    [SerializeField]
    AudioClip lockComboClip;
    public AudioClip GetLockClip => lockComboClip;

    [SerializeField]
    AudioClip unlockComboClip;
    public AudioClip GetUnlockClip => unlockComboClip;

    [SerializeField]
    AudioClip comboButtonPressedClip;
    public AudioClip GetComboPressedClip => comboButtonPressedClip;

    [Header("Colors")]

    [SerializeField]
    Color unlockedButtonColor = Color.green;

    [SerializeField]
    Color incorrectComboButtonColor = Color.red;

    //Attributes
    const string DEFAULT_INPUT_TEXT = "0000";
    string userInput = "";

    //Called on first frame update
    void Start()
    {
        //Tell all the combo buttons to listen to a method when they are selected
        for (int i = 0; i < comboButtons.Length; i++)
        {
            comboButtons[i].selectEntered.AddListener(OnComboButtonPressed);
        }

        //Set input text to default
        textInput.text = DEFAULT_INPUT_TEXT;
    }

    void OnComboButtonPressed(SelectEnterEventArgs arg0)
    {
        //Use SelectEnterEventArgs
        for (int i = 0; i < comboButtons.Length; i++)
        {
            //Find the object's name in the array to retrieve the index/number for the object
            if (arg0.interactableObject.transform.name == comboButtons[i].transform.name)
            {
                userInput += i.ToString();
                textInput.text = userInput;
                if (userInput.Length == numberCombination.Length)
                {
                    CheckCombination();
                }
                else
                {
                    OnComboButtonPress();
                }
            }
            //Reset the color of other objects to normal color when button is pressed
            comboButtons[i].SetColorToNormal();
        }
    }

    void CheckCombination()
    {
        //Check if the combination is correct. If it is, prevent the buttons from being interacted with,
        //isLocked is false, & set the button colors to a green color for user feedback

        //Debug.Log($"Combination Code: {numberCombination}");
        //Debug.Log($"User Guessed: {userInput}");
        //Debug.Log(numberCombination.CompareTo(userInput));

        if (numberCombination.CompareTo(userInput) == 0)
        {
            //Debug.Log("You found the combination");
            isLocked = false;
            OnUnlocked();
            for (int i = 0;i < comboButtons.Length; i++)
            {
                comboButtons[i].GetComponent<ButtonInteractable>().interactionLayers = InteractionLayerMask.GetMask("Nothing");
                Invoke("SetButtonsToUnlockedColor", .1f);
            }
        }
        //Combination has not been found. Reset the string & flash the buttons red for user feedback
        else
        {
            OnLocked();
            for(int i = 0; i < comboButtons.Length; i++)
            {
                comboButtons[i].SetButtonColor(incorrectComboButtonColor);
            }

            Invoke("ResetCombination", 0.5f);
        }
    }

    //Invoked method to reset the combination when the user enters invalid combo
    void ResetCombination()
    {
        for (int i = 0; i < comboButtons.Length; i++)
        {
            comboButtons[i].SetColorToNormal();
        }
        textInput.text = DEFAULT_INPUT_TEXT;
        userInput = string.Empty;
    }

    //Invoked method to set the button colors to the unlocked color when the user enters a valid combo
    void SetButtonsToUnlockedColor()
    {
        for (int i = 0; i < comboButtons.Length; i++)
        {
            comboButtons[i].SetButtonColor(unlockedButtonColor);
        }
    }
}
