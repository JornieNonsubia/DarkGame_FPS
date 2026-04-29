using Godot;
using System;

public partial class AirborneState : PlayerStateMachine
{
    private void OnAirborneStatePhysicsProcessing(double delta)
    {
        if (PlayerController.IsOnFloor())
            PlayerController.stateChart.Call("send_event", "onGrounded");
    }
}