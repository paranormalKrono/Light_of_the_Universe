﻿using System.Collections;
using UnityEngine;

public abstract class Main_Mission : MonoBehaviour
{
    [SerializeField] protected Scenes SpaceMission;
    [SerializeField] protected ScenesLocations sceneLocationName;
    [SerializeField] protected TextAsset textAsset;
    [SerializeField] protected AudioClip audioClip;

    protected Player_Starship_Controller playerController;
    protected Player_Camera_Controller playerCamera;

    protected bool isEndOrRestart;


    private void Awake()
    {
        GameManager.Initialize();
        GameScreenDark.SetDarkEvent(true);
        SceneController.LoadAdditiveScene(sceneLocationName);

        GameText.DeactivateEvent();
        GameText.SetInGameTextNowEvent(textAsset);

        playerCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Player_Camera_Controller>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_Starship_Controller>();
        playerController.GetComponent<Health>().OnDeath += RestartGame;
    }

    private void Start()
    {
        MStart();

        if (!StaticSettings.isRestart)
        {
            SetGameStop(true);
            GameAudio.StartAudioEvent(audioClip, true);
            GameDialogs.StartDialogEvent(StartGame);
        }
        else
        {
            StaticSettings.isRestart = false;
            MRestart();
        }
        playerCamera.UpdatePlayerLookPosition();

        StartCoroutine(GameScreenDark.ITransparentEvent());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (!isEndOrRestart)
            {
                EndGame();
            }
        }
    }

    protected virtual void MRestart() { }

    protected virtual void MStart() { }

    protected abstract void StartGame();

    protected void RestartGame()
    {
        if (!isEndOrRestart)
        {
            isEndOrRestart = true;
            StartCoroutine(IRestartGame());
        }
    }

    protected void EndGame()
    {
        if (!isEndOrRestart)
        {
            isEndOrRestart = true;
            StartCoroutine(IEndGame());
        }
    }


    protected abstract void SetGameStop(bool t);

    protected abstract IEnumerator IRestartGame();

    protected abstract IEnumerator IEndGame();

}