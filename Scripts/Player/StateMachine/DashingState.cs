using Godot;
using System;

public partial class DashingState : PlayerStateMachine
{
    [Export] private float _dashDuration = 0.2f; // Тривалість ривка в секундах
    private float _dashTimer = 0f;

    void OnDashingStateEntered()
    {
        PlayerController.Dash();
        _dashTimer = _dashDuration;
        GD.Print("Dash Entered");
    }

    private void OnDashingStatePhysicsProcessing(double delta)
    {
        if (_dashTimer > 0)
        {
            _dashTimer -= (float)delta;
            if (_dashTimer <= 0)
                PlayerController.stateChart.Call("send_event", "onWalking");
            GD.Print("Dash Proc");
        }
    }

    private void OnDashingStateExited()
    {
        GD.Print("Dash Exited");
        Vector3 currentVel = PlayerController.Velocity;
        PlayerController.Velocity = new Vector3(currentVel.X * 0.2f, currentVel.Y, currentVel.Z * 0.2f);
    }
}