using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRAudioManager : MonoBehaviour
{
    [Header("Progress Control")]
    [SerializeField] private ProgressControl progressControl;
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioClip fallBackClip;
    [SerializeField] private AudioSource progressSound;
    [SerializeField] private AudioClip startGameClip;
    [SerializeField] private AudioClip challengeCompleteClip;

    [Header("Grab Interactables")]
    [SerializeField] private XRGrabInteractable[] grabInteractables;
    [SerializeField] private AudioSource grabSound;
    [SerializeField] private AudioSource activatedSound;
    [SerializeField] private AudioClip grabClip;
    [SerializeField] private AudioClip keyClip;
    [SerializeField] private AudioClip grabActivatedClip;
    [SerializeField] private AudioClip wandActivatedClip;

    [Header("Drawer Interactables")]
    [SerializeField] private DrawerInteractable drawer;
    private XRSocketInteractor drawerSocket;
    private AudioSource drawerSound;
    private AudioSource drawerSocketSound;
    private AudioClip drawerMoveClip;
    private AudioClip drawerSocketClip;

    [Header("Hinge Interactables")]
    [SerializeField] private SimpleHingeInteractable[] cabinetDoors = new SimpleHingeInteractable[2];
    private AudioSource[] cabinetDoorSound;
    private AudioClip cabinetDoorMoveClip;

    [Header("Combo Lock")]
    [SerializeField] private CombinationLock comboLock;
    private AudioSource comboLockSound;
    private AudioClip lockComboClip;
    private AudioClip unlockComboClip;
    private AudioClip comboButtonPressedClip;

    [Header("The Wall")]
    [SerializeField] private TheWall wall;
    [SerializeField] private AudioSource wallSound;
    private XRSocketInteractor wallSocket;
    private AudioSource wallSocketSound;
    private AudioClip destroyWallClip;
    private AudioClip wallSocketClip;

    private bool startAudioBool;

    private void OnEnable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.AddListener(StartGame);
            progressControl.OnChallengeComplete.AddListener(ChallengeComplete);
        }

        if (fallBackClip == null)
        {
            fallBackClip = AudioClip.Create("FallBack", 1, 1, 1000, false);
        }

        SetGrabbables();

        if (drawer != null) SetDrawerInteractable();
        if (wall != null) SetWall();

        cabinetDoorSound = new AudioSource[cabinetDoors.Length];
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (cabinetDoors[i] != null)
            {
                SetCabinetDoors(i);
            }
        }

        if (comboLock != null) SetComboLock();
    }

    private void OnDisable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.RemoveListener(StartGame);
            progressControl.OnChallengeComplete.RemoveListener(ChallengeComplete);
        }

        if (grabInteractables != null)
        {
            foreach (var grabbable in grabInteractables)
            {
                grabbable.selectEntered.RemoveListener(OnSelectEnteredGrabbable);
                grabbable.selectExited.RemoveListener(OnSelectExitedGrabbable);
                grabbable.activated.RemoveListener(OnActivatedGrabbable);
            }
        }

        if (wall != null)
        {
            wall.OnDestroy.RemoveListener(OnDestroyWall);
            if (wallSocket != null)
                wallSocket.selectEntered.RemoveListener(OnWallSocketed);
        }

        if (comboLock != null)
        {
            comboLock.UnlockAction -= OnComboUnlocked;
            comboLock.LockAction -= OnComboLocked;
            comboLock.ComboButtonPressed -= OnComboButtonPressed;
        }

        if (drawer != null)
        {
            drawer.selectEntered.RemoveListener(OnDrawerMove);
            drawer.selectExited.RemoveListener(OnDrawerStop);

            if (drawerSocket != null)
                drawerSocket.selectEntered.RemoveListener(OnDrawerSocketed);
        }

        foreach (var door in cabinetDoors)
        {
            if (door != null)
            {
                door.OnHingeSelected.RemoveListener(OnDoorMove);
                door.selectExited.RemoveListener(OnDoorStop);
            }
        }
    }

    void SetCabinetDoors(int index)
    {
        cabinetDoorSound[index] = cabinetDoors[index].gameObject.AddComponent<AudioSource>();
        cabinetDoorMoveClip = cabinetDoors[index].HingeMoveClip;  // Changed to property access
        CheckIfClipIsNull(ref cabinetDoorMoveClip);

        cabinetDoorSound[index].clip = cabinetDoorMoveClip;
        cabinetDoorSound[index].loop = true;

        cabinetDoors[index].OnHingeSelected.AddListener(OnDoorMove);
        cabinetDoors[index].selectExited.AddListener(OnDoorStop);
    }


    private void ChallengeComplete(string arg0)
    {
        if (progressSound != null && challengeCompleteClip != null)
        {
            progressSound.clip = challengeCompleteClip;
            progressSound.Play();
        }
    }

    void SetComboLock()
    {
        comboLockSound = comboLock.gameObject.AddComponent<AudioSource>();

        lockComboClip = comboLock.LockClip;  // Changed to property access
        unlockComboClip = comboLock.UnlockClip;  // Changed to property access
        comboButtonPressedClip = comboLock.ComboPressedClip;  // Changed to property access

        CheckIfClipIsNull(ref lockComboClip);
        CheckIfClipIsNull(ref unlockComboClip);
        CheckIfClipIsNull(ref comboButtonPressedClip);

        comboLock.UnlockAction += OnComboUnlocked;
        comboLock.LockAction += OnComboLocked;
        comboLock.ComboButtonPressed += OnComboButtonPressed;
    }


    private void OnComboButtonPressed()
    {
        comboLockSound.clip = comboButtonPressedClip;
        comboLockSound.Play();
    }

    private void OnComboLocked()
    {
        comboLockSound.clip = lockComboClip;
        comboLockSound.Play();
    }

    private void OnComboUnlocked()
    {
        comboLockSound.clip = unlockComboClip;
        comboLockSound.Play();
    }

    private void OnDoorStop(SelectExitEventArgs arg0)
    {
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (arg0.interactableObject == cabinetDoors[i])
            {
                cabinetDoorSound[i]?.Stop();
            }
        }
    }

    private void OnDoorMove(SimpleHingeInteractable arg0)
    {
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (arg0 == cabinetDoors[i])
            {
                cabinetDoorSound[i]?.Play();
            }
        }
    }

    private void OnSelectEnteredGrabbable(SelectEnterEventArgs arg0)
    {
        grabSound.clip = arg0.interactableObject.transform.CompareTag("Key") ? keyClip : grabClip;
        grabSound.Play();
    }

    private void OnSelectExitedGrabbable(SelectExitEventArgs arg0)
    {
        grabSound.clip = grabClip;
        grabSound.Play();
    }

    private void OnActivatedGrabbable(ActivateEventArgs arg0)
    {
        var temp = arg0.interactableObject.transform.gameObject;
        activatedSound.clip = temp.GetComponent<WandControl>() != null ? wandActivatedClip : grabActivatedClip;
        activatedSound.Play();
    }

    private void OnDestroyWall()
    {
        if (wallSound != null && destroyWallClip != null)
        {
            wallSound.clip = destroyWallClip;
            wallSound.Play();
        }
    }

    private void OnDrawerStop(SelectExitEventArgs arg0) => drawerSound?.Stop();
    private void OnDrawerMove(SelectEnterEventArgs arg0) => drawerSound?.Play();
    private void OnWallSocketed(SelectEnterEventArgs arg0) => wallSocketSound?.Play();
    private void OnDrawerSocketed(SelectEnterEventArgs arg0) => drawerSocketSound?.Play();

    void SetGrabbables()
    {
        if (grabInteractables == null || grabInteractables.Length == 0)
        {
            grabInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);
        }

        foreach (var grabbable in grabInteractables)
        {
            grabbable.selectEntered.AddListener(OnSelectEnteredGrabbable);
            grabbable.selectExited.AddListener(OnSelectExitedGrabbable);
            grabbable.activated.AddListener(OnActivatedGrabbable);
        }
    }

    private void SetWall()
    {
        destroyWallClip = wall.DestroyClip;  // Changed to property access
        CheckIfClipIsNull(ref destroyWallClip);
        wall.OnDestroy.AddListener(OnDestroyWall);

        wallSocket = wall.WallSocket;  // Changed to property access
        if (wallSocket != null)
        {
            wallSocketSound = wallSocket.gameObject.AddComponent<AudioSource>();
            wallSocketClip = wall.SocketClip;  // Changed to property access
            CheckIfClipIsNull(ref wallSocketClip);
            wallSocketSound.clip = wallSocketClip;
            wallSocket.selectEntered.AddListener(OnWallSocketed);
        }
    }


    private void SetDrawerInteractable()
    {
        drawerSound = drawer.gameObject.AddComponent<AudioSource>();
        drawerMoveClip = drawer.MoveClip;  // Changed to property access
        CheckIfClipIsNull(ref drawerMoveClip);
        drawerSound.clip = drawerMoveClip;
        drawerSound.loop = true;

        drawer.selectEntered.AddListener(OnDrawerMove);
        drawer.selectExited.AddListener(OnDrawerStop);

        drawerSocket = drawer.SocketIntractor;  // Changed to property access
        if (drawerSocket != null)
        {
            drawerSocketSound = drawerSocket.gameObject.AddComponent<AudioSource>();
            drawerSocketClip = drawer.SocketedClip;  // Changed to property access
            CheckIfClipIsNull(ref drawerSocketClip);
            drawerSocketSound.clip = drawerSocketClip;
            drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
        }
    }

    private void StartGame(string arg0)
    {
        if (!startAudioBool)
        {
            startAudioBool = true;

            if (backgroundMusic != null && startGameClip != null)
            {
                backgroundMusic.clip = startGameClip;
                backgroundMusic.Play();
            }
        }
        else
        {
            if (progressSound != null && startGameClip != null)
            {
                progressSound.clip = startGameClip;
                progressSound.Play();
            }
        }
    }

    void CheckIfClipIsNull(ref AudioClip clip)
    {
        if (clip == null)
        {
            clip = fallBackClip;
        }
    }
}
