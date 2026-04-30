public partial class IdleState : PlayerStateMachine
{
    private void OnIdleStatePhysicsProcessing(double delta)
    {
        if (_player != null && _player.InputDir.Length() > 0)
        {
            _player.StateChart.Call("send_event", "onMoving");
        }
    }
}