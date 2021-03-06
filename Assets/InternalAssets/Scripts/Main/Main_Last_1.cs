﻿using System.Collections;
using UnityEngine;

public class Main_Last_1 : MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private AudioClip audioClip;

    [SerializeField] private Starship_AI_Adv Z2StarshipAI;

    [SerializeField] private GameObject Z2Image;
    [SerializeField] private Transform Target;

    private Player_Starship_Controller playerController;
    private Player_Camera_Controller playerCamera;

    private Transform PlayerStarshipTr;

    private bool isRestart;
    private bool isEnd;


    private void Awake()
    {
        GameManager.Initialize();

        GameText.DeactivateEvent();
        GameText.SetInGameTextNowEvent(textAsset);

        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Starship_Controller>();
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();

        PlayerStarshipTr = playerController.transform;
    }

    private void Start()
    {
        GameAudio.StartAudioEvent(audioClip, 0.4f, true);

        playerController.SetLockControl(true);
        playerCamera.SetLockMove(false);
        Z2StarshipAI.SetLockControl(true);

        playerCamera.SetPositionWithOffset(PlayerStarshipTr.position);

        playerCamera.EnableTargetMove(Target);

        playerController.SetActiveCanvas(false);

        Z2Image.SetActive(true);

        GameDialogs.StartDialogEvent(OnStartDialogEnd);
    }

    private void FixedUpdate()
    {
        if (!isEnd)
        {
            if (Input.GetKey(KeyCode.N))
            {
                EndGame();
            }
        }
    }

    private void OnStartDialogEnd() => EndGame();

    private void EndGame()
    {
        if (!isRestart && !isEnd)
        {
            isEnd = true;
            SceneController.LoadNextStoryScene();
        }
    }
}
