using Godot;

public partial class MovingState : PlayerStateMachine
{
    void OnMovingStatePhysicsProcessing(double delta)
    {
        if (_player != null && _player.InputDir.Length() == 0 &&
            _player.Velocity.Length() < 0.5)
        {
            _player.StateChart.Call("send_event", "onIdle");
        }

        if (Input.IsActionJustPressed("Dash") && _player!.CanDash)
        {
            _player.StateChart.Call("send_event", "onDashing");
        }
    }
}