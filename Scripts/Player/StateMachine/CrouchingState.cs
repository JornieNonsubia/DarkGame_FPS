using Godot;
using System;

public partial class CrouchingState : PlayerStateMachine
{
    void OnCrouchingStatePhysicsProcessing(double delta)
    {
        PlayerController.Head.UpdateCameraHeight(delta,-1);
        if (!Input.IsActionPressed("Crouch") && PlayerController.IsOnFloor() && !PlayerController.CrouchCheck.IsColliding())

            PlayerController.StateChart.Call("send_event", "onStanding");
    }

    void OnCrouchingStateEntered()
    {
        PlayerController.Crouch();
    }
}