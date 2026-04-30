using Godot;

public partial class SprintingState : PlayerStateMachine
{
    void OnSprintingStatePhysicsProcessing(double delta)
    {
        if (!Input.IsActionPressed("Sprint"))
            _player.StateChart.Call("send_event", "onWalking");
    }

    void OnSprintingStateEntered()
    {
        _player.Sprint();
    }
}