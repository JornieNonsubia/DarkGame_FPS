using Godot;
using System;

public partial class AirborneState : PlayerStateMachine
{
    private void OnAirborneStatePhysicsProcessing(double delta)
    {
        if (PlayerController.IsOnFloor())
        {
            if (PlayerController.CheckFallSpeed())
                PlayerController.CameraEffects.AddFallKick(2f);
            PlayerController.stateChart.Call("send_event", "onGrounded");
        }
        PlayerController.CurrentFallVelocity = PlayerController.Velocity.Y;
    }
}