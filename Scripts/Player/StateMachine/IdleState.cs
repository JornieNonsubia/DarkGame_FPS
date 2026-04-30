using Godot;
using System;

public partial class IdleState : PlayerStateMachine
{
    private void OnIdleStatePhysicsProcessing(double delta)
    {
        if (PlayerController != null && PlayerController.InputDir.Length() > 0)
        {
            PlayerController.StateChart.Call("send_event", "onMoving");
        }
    }
}