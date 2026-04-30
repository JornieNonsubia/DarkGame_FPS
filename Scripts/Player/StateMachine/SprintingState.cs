using Godot;
using System;

public partial class SprintingState : PlayerStateMachine
{
    void OnSprintingStatePhysicsProcessing(double delta)
    {
        if (!Input.IsActionPressed("Sprint"))
            PlayerController.StateChart.Call("send_event", "onWalking");
    }

    void OnSprintingStateEntered()
    {
        PlayerController.Sprint();
    }
}