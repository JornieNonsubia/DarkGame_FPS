using Godot;

public partial class CrouchingState : PlayerStateMachine
{
    void OnCrouchingStatePhysicsProcessing(double delta)
    {
        _player.Head.UpdateCameraHeight(delta, -1);

        if (!Input.IsActionPressed("Crouch") && _player.IsOnFloor() && !_player.CrouchCheck.IsColliding())

            _player.StateChart.Call("send_event", "onStanding");
    }

    void OnCrouchingStateEntered()
    {
        _player.Crouch();
    }
}