using System.Collections;
using UnityEngine;

[RequireComponent(typeof (System_TimeRace), typeof (System_Starships), typeof(StarshipsSpawnMover))]
public class Main_Space_TimeRace : Main_Mission
{
    private System_TimeRace system_TimeRace;

    protected override void MStart()
    {
        system_TimeRace = GetComponent<System_TimeRace>();
        GetComponent<StarshipsSpawnMover>().MoveStarshipsOnSpawns();
    }

    protected override void MRestart()
    {
        StartCoroutine(IRestart());

    }

    private IEnumerator IRestart()
    {
        SetGameStop(true);
        GameGoals.SetActiveGoalEvent(true);
        system_TimeRace.ShowTimer();
        yield return GameFreezeTime.IFreezeTimeEvent();
        system_TimeRace.Activate(RestartGame, EndGame);
        SetGameStop(false);
    }

    protected override void StartGame()
    {
        GameGoals.SetActiveGoalEvent(true);
        system_TimeRace.Activate(RestartGame, EndGame);
        SetGameStop(false);
    }


    protected override IEnumerator IRestartGame()
    {
        GameTimer.StopTimerEvent();
        GameWinLose.ActivateLoseTextEvent();
        yield return new WaitForSeconds(2);
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

        GameTimer.DeactivateEvent();

        GameWinLose.DisactivateTextEvent();
        GameReward.HideRewardEvent();

        SceneController.LoadNextStoryScene();
    }


    protected override void SetGameStop(bool t)
    {
        playerController.SetLockControl(t);
        playerCamera.SetLockMove(t);
    }
}