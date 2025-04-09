using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SimpleUIControl : MonoBehaviour
{
    [SerializeField] ProgressControl progressControl;

    //Serialized Fields
    [Header("Interactables")]
    [SerializeField]
    ButtonInteractable buttonInteractable;



    [Header("String Handling")]



    [SerializeField]
    TMP_Text[] messageTexts;

    void OnEnable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.AddListener(StartGame);
            progressControl.OnChallengeComplete.AddListener(ChallengeComplete);
        }
    }

    private void ChallengeComplete(string arg0)
    {
        SetText(arg0);
    }

    private void StartGame(string arg0)
    {
        SetText(arg0);
    }





    public void SetText(string message)
    {
        //Update all text elements that should change when button is pressed.
        for (int i = 0; i < messageTexts.Length; i++)
        {
            messageTexts[i].text = message;
        }
    }

}



  