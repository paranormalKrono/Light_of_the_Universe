using UnityEngine;

public class Main_Prolog_2 : Main_Prolog
{
    [SerializeField] private TimeTeach timeTeach1;
    [SerializeField] private TimeTeach timeTeach2;
    [SerializeField] private PlayerStarshipTrigger Checkpoint1Trigger;
    [SerializeField] private PlayerStarshipTrigger Checkpoint2Trigger;
    [SerializeField] private Transform CheckpointTr1;
    [SerializeField] private Transform CheckpointTr2;
    [SerializeField] private PlayerStarshipTrigger Dialog0Trigger;
    [SerializeField] private PlayerStarshipTrigger Dialog1Trigger;
    [SerializeField] private PlayerStarshipTrigger Dialog2Trigger;
    [SerializeField] private PlayerStarshipTrigger Dialog3Trigger;

    protected override void Checkpoint(int checkpointID)
    {
        if (checkpointID == 0)
        {
            Dialog0Trigger.OnPlayerStarshipEnter += ShowDialog0;
            Dialog1Trigger.OnPlayerStarshipEnter += ShowDialog1;

            Checkpoint1Trigger.OnPlayerStarshipEnter += DoCheckpoint1;
            Checkpoint2Trigger.OnPlayerStarshipEnter += DoCheckpoint2;
            timeTeach1.Activate();
            timeTeach2.Activate();
            timeTeach1.OnLoseTeach += Restart;
            timeTeach2.OnLoseTeach += Restart;
        }
        else if (checkpointID == 1)
        {
            Dialog1Trigger.OnPlayerStarshipEnter += ShowDialog1;

            timeTeach1.InstantlyTeach();

            Checkpoint2Trigger.OnPlayerStarshipEnter += DoCheckpoint2;
            timeTeach2.Activate();
            timeTeach2.OnLoseTeach += Restart;

            MovePlayerToCheckpoint(CheckpointTr1);
            player_Camera_Controller.SetPositionWithOffset(CheckpointTr1.position);
        }
        else if (checkpointID == 2)
        {
            timeTeach1.InstantlyTeach();
            timeTeach2.InstantlyTeach();
            MovePlayerToCheckpoint(CheckpointTr2);
            player_Camera_Controller.SetPositionWithOffset(CheckpointTr2.position);
        }

        Dialog2Trigger.OnPlayerStarshipEnter += ShowDialog2;
        Dialog3Trigger.OnPlayerStarshipEnter += ShowDialog3;
    }

    private void DoCheckpoint1()
    {
        Checkpoint1Trigger.OnPlayerStarshipEnter -= DoCheckpoint1;
        SetCheckpoint(1);
    }
    private void DoCheckpoint2()
    {
        Checkpoint2Trigger.OnPlayerStarshipEnter -= DoCheckpoint2;
        SetCheckpoint(2);
    }

    private void ShowDialog0()
    {
        Dialog0Trigger.OnPlayerStarshipEnter -= ShowDialog0;
        GameDialogs.ShowInGameDialogEvent(0);
    }
    private void ShowDialog1()
    {
        Dialog1Trigger.OnPlayerStarshipEnter -= ShowDialog1;
        GameDialogs.ShowInGameDialogEvent(1);
    }
    private void ShowDialog2()
    {
        Dialog2Trigger.OnPlayerStarshipEnter -= ShowDialog2;
        GameDialogs.ShowInGameDialogEvent(2);
    }
    private void ShowDialog3()
    {
        Dialog3Trigger.OnPlayerStarshipEnter -= ShowDialog3;
        GameDialogs.ShowInGameDialogEvent(3);
    }
}
