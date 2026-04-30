public partial class OnGroundedState : PlayerStateMachine
{
    private void OnGroundedStatePhysicsProcessing(double delta)
    {
        if (!_player.IsOnFloor())
            _player.StateChart.Call("send_event", "onAirborne");
    }
}