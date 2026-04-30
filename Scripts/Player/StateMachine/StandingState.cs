using Godot;

public partial class StandingState : PlayerStateMachine
{
    void OnStandingStatePhysicsProcessing(double delta)
    {
        _player.Head.UpdateCameraHeight(delta, 1);

        if (Input.IsActionPressed("Crouch") && _player.IsOnFloor())
        {
            _player.StateChart.Call("send_event", "onCrouching");
        }
    }

    void OnStandingStateEntered()
    {
        _player.Stand();
    }
}