using System.Collections;
using UnityEngine;

[RequireComponent(typeof(System_Starships), typeof(StarshipsSpawnMover))]
public class Main_Space : Main_Mission
{
    [SerializeField] private Scenes nextSceneToLoad;
    [SerializeField] private int nextEquationsID;
    [SerializeField] private bool isEquationsAndSlides;
    [SerializeField] private int nextSlidesID;
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
        yield return StartCoroutine(ScreenDark.IDarkEvent());
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

        GameAudio.StopAudioEvent();

        yield return StartCoroutine(ScreenDark.IDarkEvent());

        GameWinLose.DisactivateTextEvent();
        GameReward.HideRewardEvent();

        StaticSettings.GameProgress = (int)SpaceMission + 1;

        if (nextSceneToLoad == Scenes.Space_Base)
        {
            StaticSettings.MissionStagesProgress = SceneController.GetMissionStage(SpaceMission) + 1;
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