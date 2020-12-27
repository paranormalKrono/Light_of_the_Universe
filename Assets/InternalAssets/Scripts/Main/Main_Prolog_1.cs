using UnityEngine;

public class Main_Prolog_1 : Main_Prolog
{
    [SerializeField] private Lift lift;
    [SerializeField] private PlayerStarshipTrigger liftTrigger;
    [SerializeField] private PlayerStarshipTrigger Dialog0Trigger;
    [SerializeField] private PlayerStarshipTrigger Dialog1Trigger;
    [SerializeField] private PlayerStarshipTrigger Dialog2Trigger;

    protected override void MStart()
    {
        Dialog0Trigger.OnPlayerStarshipEnter += ShowDialog0;
        Dialog1Trigger.OnPlayerStarshipEnter += ShowDialog1;
        Dialog2Trigger.OnPlayerStarshipEnter += ShowDialog2;
        liftTrigger.OnPlayerStarshipEnter += StartLift;
    }

    private void StartLift()
    {
        player_Starship_Controller.SetLockControl(true);
        lift.StartLift(player_Starship_Controller, LiftArrived);
    }

    private void LiftArrived()
    {
        player_Starship_Controller.SetLockControl(false);
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
}
