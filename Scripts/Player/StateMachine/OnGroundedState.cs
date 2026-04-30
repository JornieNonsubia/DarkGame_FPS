using Godot;
using System;

public partial class OnGroundedState : PlayerStateMachine
{
    private void OnGroundedStatePhysicsProcessing(double delta)
    {
        if (!PlayerController.IsOnFloor())
            PlayerController.StateChart.Call("send_event", "onAirborne");
    }
}