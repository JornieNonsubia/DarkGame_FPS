using Godot;
using System;

public partial class OnGroundedState : PlayerStateMachine
{
    private void OnGroundedStatePhysicsProcessing(double delta)
    {
        if (!PlayerController.IsOnFloor())
            PlayerController.stateChart.Call("send_event", "onAirborne");
    }
}