using System.Collections;
using UnityEngine;

[RequireComponent(typeof(System_Starships), typeof(StarshipsSpawnMover))]
public class Main_Space : Main_Mission
{
    private System_Starships systemStarships;

    protected override void MStart()
    {
        systemStarships = GetComponent<System_Starships>();
        systemStarships.InitializeStarshipsTeams(GetComponent<StarshipsSpawnMover>().MoveStarshipsOnSpawns());
        systemStarships.OnOneTeamLeft += EndGame;
    }

    protected override void MRestart()
    {
        GameGoals.SetActiveGoalEvent(true);
        systemStarships.SetStarshipsLock(false);
    }

    protected override void StartGame()
    {
        GameGoals.SetActiveGoalEvent(true);
        SetGameStop(false);
    }


    protected override IEnumerator IRestartGame()
    {
        GameWinLose.ActivateLoseTextEvent();
        yield return new WaitForSeconds(2);
        GameWinLose.DisactivateTextEvent();
        SceneController.RestartScene();
    }

    protected override IEnumerator IEndGame()
    {
        GameWinLose.ActivateWinTextEvent();

        int reward = SpaceMissions.GetSpaceMissionData(SpaceMission).reward;
        GameReward.ShowRewardEvent(reward);
        StaticSettings.credits += reward;

        yield return new WaitForSeconds(3);

        GameWinLose.DisactivateTextEvent();
        GameReward.HideRewardEvent();

        SceneController.LoadNextStoryScene();
    }

    protected override void SetGameStop(bool t)
    {
        playerController.SetLockControl(t);
        playerCamera.SetLockMove(t);
        systemStarships.SetStarshipsLock(t);
    }
}