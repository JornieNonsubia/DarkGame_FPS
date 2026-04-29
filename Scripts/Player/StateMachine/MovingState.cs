using Godot;
using System;

public partial class MovingState : PlayerStateMachine
{
    void OnMovingStatePhysicsProcessing(double delta)
    {
        if (PlayerController != null && PlayerController.InputDir.Length() == 0 &&
            PlayerController.Velocity.Length() < 0.5)
        {
            PlayerController.stateChart.Call("send_event", "onIdle");
        }
        if (Input.IsActionJustPressed("Dash"))
        {
            PlayerController.stateChart.Call("send_event", "onDashing");
        }
    }
}