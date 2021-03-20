using UnityEngine;

public class Main_Prolog_3 : Main_Prolog
{
    [SerializeField] private Color newEmissionLightColor;
    [SerializeField] private PhysicButton physicButton;
    [SerializeField] private Door EndDoor;
    [SerializeField] private Activator EnemiesActivator;
    [SerializeField] private EmissionColorChanger colorChanger;
    [SerializeField] private Event_Arena Arena;
    [SerializeField] private Event_Targets Targets;
    [SerializeField] private Transform CheckpointTr1;
    [SerializeField] private Transform CheckpointTr2;
    [SerializeField] private Transform CheckpointTr3;
    [SerializeField] private Transform CheckpointTr4;
    [SerializeField] private PlayerStarshipTrigger Checkpoint1Trigger;
    [SerializeField] private PlayerStarshipTrigger Checkpoint2Trigger;
    [SerializeField] private PlayerStarshipTrigger Checkpoint3Trigger;
    [SerializeField] private PlayerStarshipTrigger Dialog4Trigger;
    [SerializeField] private PlayerStarshipTrigger Dialog5Trigger;
    [SerializeField] private PlayerStarshipTrigger Dialog6Trigger;
    [SerializeField] private PlayerStarshipTrigger Dialog7Trigger;


    protected override void Checkpoint(int checkpointID)
    {
        switch (checkpointID)
        {
            case 0:
                Checkpoint1Trigger.OnPlayerStarshipEnter += DoCheckpoint1;
                Targets.OnStart += OnTargets;
                Targets.OnEnd += OnTargetsEnd;
                break;
            case 1:
                GameGoals.ShowGoalEvent(2);

                MovePlayerToCheckpoint(CheckpointTr1);
                player_Camera_Controller.SetPositionWithOffset(CheckpointTr1.position);

                Checkpoint2Trigger.OnPlayerStarshipEnter += DoCheckpoint2;
                Arena.OnStart += OnArenaStart;
                Arena.OnEnd += OnArenaEnd;

                break;
            case 2:
                GameGoals.ShowGoalEvent(4);

                MovePlayerToCheckpoint(CheckpointTr2);
                player_Camera_Controller.SetPositionWithOffset(CheckpointTr2.position);

                Dialog4Trigger.OnPlayerStarshipEnter += Dialog4;
                Dialog5Trigger.OnPlayerStarshipEnter += Dialog5;
                Checkpoint3Trigger.OnPlayerStarshipEnter += DoCheckpoint3;

                break;
            case 3:
                GameGoals.ShowGoalEvent(5);
                MovePlayerToCheckpoint(CheckpointTr3);
                player_Camera_Controller.SetPositionWithOffset(CheckpointTr3.position);

                Dialog6Trigger.OnPlayerStarshipEnter += Dialog6;

                physicButton.OnButtonPushed += OnButtonPushed;
                break;
            case 4:
                OnButtonPushed();
                MovePlayerToCheckpoint(CheckpointTr4);
                player_Camera_Controller.SetPositionWithOffset(CheckpointTr4.position);
                break;
        }
    }
    private void DoCheckpoint1()
    {
        Checkpoint1Trigger.OnPlayerStarshipEnter -= DoCheckpoint1;

        Checkpoint2Trigger.OnPlayerStarshipEnter += DoCheckpoint2;
        Arena.OnStart += OnArenaStart;
        Arena.OnEnd += OnArenaEnd;

        SetCheckpoint(1);
    }
    private void DoCheckpoint2()
    {
        Checkpoint2Trigger.OnPlayerStarshipEnter -= DoCheckpoint2;

        Dialog4Trigger.OnPlayerStarshipEnter += Dialog4;
        Dialog5Trigger.OnPlayerStarshipEnter += Dialog5;
        Checkpoint3Trigger.OnPlayerStarshipEnter += DoCheckpoint3;

        SetCheckpoint(2);
    }
    private void DoCheckpoint3()
    {
        Checkpoint3Trigger.OnPlayerStarshipEnter -= DoCheckpoint3;

        Dialog6Trigger.OnPlayerStarshipEnter += Dialog6;

        physicButton.OnButtonPushed += OnButtonPushed;

        GameGoals.ShowGoalEvent(5);

        SetCheckpoint(3);
    }
    private void OnButtonPushed()
    {
        Dialog5Trigger.OnPlayerStarshipEnter -= Dialog5;
        physicButton.OnButtonPushed -= OnButtonPushed;

        Dialog7Trigger.OnPlayerStarshipEnter += Dialog7;

        EndDoor.Move();
        EnemiesActivator.SetActiveGameObjects(true);
        colorChanger.ChangeEmissionColor(newEmissionLightColor);
        GameGoals.ShowGoalEvent(6);

        SetCheckpoint(4);
    }

    private void OnTargets()
    {
        Targets.OnStart -= OnTargets;
        GameDialogs.ShowInGameDialogEvent(0);
        GameGoals.ShowGoalEvent(1);
    }
    private void OnTargetsEnd()
    {
        Targets.OnEnd -= OnTargetsEnd;
        GameDialogs.ShowInGameDialogEvent(1);
        GameGoals.ShowGoalEvent(2);
    }
    private void OnArenaStart()
    {
        Arena.OnStart -= OnArenaStart;
        GameDialogs.ShowInGameDialogEvent(2);
        GameGoals.ShowGoalEvent(3);
    }
    private void OnArenaEnd()
    {
        Arena.OnEnd -= OnArenaEnd;
        GameDialogs.ShowInGameDialogEvent(3);
        GameGoals.ShowGoalEvent(4);
    }
    private void Dialog4()
    {
        Dialog4Trigger.OnPlayerStarshipEnter -= Dialog4;
        GameDialogs.ShowInGameDialogEvent(4);
    }
    private void Dialog5()
    {
        Dialog5Trigger.OnPlayerStarshipEnter -= Dialog5;
        GameDialogs.ShowInGameDialogEvent(5);
        GameGoals.ShowGoalEvent(5);
    }
    private void Dialog6()
    {
        Dialog6Trigger.OnPlayerStarshipEnter -= Dialog6;
        GameDialogs.ShowInGameDialogEvent(6);
    }
    private void Dialog7()
    {
        Dialog7Trigger.OnPlayerStarshipEnter -= Dialog7;
        GameDialogs.ShowInGameDialogEvent(7);
    }

}
