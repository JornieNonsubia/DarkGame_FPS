using Godot;
using System;

public partial class StandingState : PlayerStateMachine
{
    void OnStandingStatePhysicsProcessing(double delta)
    {
        PlayerController.Head.UpdateCameraHeight(delta, 1);
        if (Input.IsActionPressed("Crouch") && PlayerController.IsOnFloor())
        {
            PlayerController.stateChart.Call("send_event", "onCrouching");
        }
    }

    void OnStandingStateEntered()
    {
        PlayerController.Stand();
    }
}