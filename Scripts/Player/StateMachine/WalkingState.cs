using Godot;
using System;

public partial class WalkingState : PlayerStateMachine
{
    void OnWalkingStatePhysicsProcessing(double delta)
    {
        if (Input.IsActionPressed("Sprint"))
        {
            PlayerController.stateChart.Call("send_event", "onSprinting");
        }
    }

    void OnWalkingStateEntered()
    {
        PlayerController.Walk();
    }
}
