using Godot;

public partial class WalkingState : PlayerStateMachine
{
    void OnWalkingStatePhysicsProcessing(double delta)
    {
        if (Input.IsActionPressed("Sprint"))
        {
            _player.StateChart.Call("send_event", "onSprinting");
        }
    }

    void OnWalkingStateEntered()
    {
        _player.Walk();
    }
}