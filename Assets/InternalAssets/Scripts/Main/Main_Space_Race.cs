using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(System_Race), typeof(System_Starships), typeof(StarshipsSpawnMover))]
public class Main_Space_Race : Main_Mission
{
    [SerializeField] private Scenes nextSceneToLoad;
    [SerializeField] private bool isEquationsAndSlides;
    [SerializeField] private int nextSlidesID;
    [SerializeField] private int nextEquationsID;
    private System_Race system_Race;
    private System_Starships systemStarships;

    protected override void MStart()
    {
        system_Race = GetComponent<System_Race>();
        systemStarships = GetComponent<System_Starships>();
        List<List<Starship>> starships = GetComponent<StarshipsSpawnMover>().MoveStarshipsOnSpawns();
        List<Transform> transforms = new List<Transform>();
        for (int i = 0; i < starships.Count; ++i)
        {
            for (int j = 0; j < starships[i].Count; ++j)
            {
                transforms.Add(starships[i][j].transform);
            }
        }
        system_Race.Initialize(transforms);
        systemStarships.InitializeStarshipsTeams(starships);
    }
    protected override void MRestart()
    {
        StartCoroutine(IRestart());

    }
    private IEnumerator IRestart()
    {
        SetGameStop(true);
        GameGoals.SetActiveGoalEvent(true);
        system_Race.ShowTimer();
        yield return GameFreezeTime.IFreezeTimeEvent();
        system_Race.StartRace(End);
        SetGameStop(false);
    }

    protected override void StartGame()
    {
        GameGoals.SetActiveGoalEvent(true);
        system_Race.StartRace(End);
        SetGameStop(false);
    }

    private void End(Transform Tr)
    {
        if (Tr.GetComponent<Player_Starship_Controller>() != null)
        {
            EndGame();
        }
        else
        {
            RestartGame();
        }
    }

    protected override IEnumerator IRestartGame()
    {
        GameTimer.StopTimerEvent();
        GameWinLose.ActivateLoseTextEvent();
        yield return new WaitForSeconds(2);
        yield return StartCoroutine(ScreenDark.IDarkEvent());
        GameWinLose.DisactivateTextEvent();
        SceneController.RestartScene();
    }

    protected override IEnumerator IEndGame()
    {
        GameTimer.StopTimerEvent();

        GameWinLose.ActivateWinTextEvent();

        int reward = SpaceMissions.GetSpaceMissionData(SpaceMission).reward;
        GameReward.ShowRewardEvent(reward);
        StaticSettings.credits += reward;

        yield return new WaitForSeconds(3);

        GameAudio.StopAudioEvent();

        yield return StartCoroutine(ScreenDark.IDarkEvent());

        GameWinLose.DisactivateTextEvent();
        GameReward.HideRewardEvent();

        GameTimer.DeactivateEvent();

        StaticSettings.GameProgress = (int)SpaceMission + 1;

        if (nextSceneToLoad == Scenes.Space_Base)
        {
            StaticSettings.MissionStagesProgress += 1;
        }
        if (isEquationsAndSlides)
        {
            SceneController.LoadEquationsAndSlides(nextSceneToLoad, nextEquationsID, nextSlidesID);
        }
        else
        {
            SceneController.LoadEquations(nextSceneToLoad, nextEquationsID);
        }
    }


    protected override void SetGameStop(bool t)
    {
        playerController.SetLockControl(t);
        playerCamera.SetLockMove(t);
        systemStarships.SetStarshipsLock(t);
    }
}