using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.Events;

public class ProgressControl : MonoBehaviour
{
    [SerializeField]
    private XRBaseInteractable buttonInteractable;

    [Header("Challenge Texts (Edit Number and Paste)")]
    [Tooltip("List of messages or challenge texts shown at different stages.")]
    [TextArea(2, 4)]
    public string[] challengeStrings;

    [Header("Game Objects and Lights")]
    [SerializeField]
    private GameObject keyInteractableLight;

    [Header("Start Options")]
    [SerializeField]
    private int challengeNumber = 0;

    [SerializeField]
    private string startGameString = "Start";

    [System.Serializable]
    public class StringEvent : UnityEvent<string> { }

    [Header("Unity Events")]
    public StringEvent OnStartGame;
    public StringEvent OnChallengeComplete;

    private bool startGameBool = false;

    void Awake()
    {
        if (buttonInteractable != null)
        {
            buttonInteractable.selectEntered.AddListener(ButtonInteractablePressed);
        }

        if (OnStartGame != null)
        {
            OnStartGame.Invoke(startGameString);
        }
    }

    void ButtonInteractablePressed(SelectEnterEventArgs arg0)
    {
        if (!startGameBool)
        {
            startGameBool = true;

            if (keyInteractableLight != null)
            {
                keyInteractableLight.SetActive(true);
            }

            if (challengeNumber < challengeStrings.Length)
            {
                OnStartGame?.Invoke(challengeStrings[challengeNumber]);
            }
        }
    }
}
