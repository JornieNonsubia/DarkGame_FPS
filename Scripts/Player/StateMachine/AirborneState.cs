public partial class AirborneState : PlayerStateMachine
{
    private void OnAirborneStatePhysicsProcessing(double delta)
    {
        if (_player.IsOnFloor())
        {
            if (_player.CheckFallSpeed())
                _player.CameraEffects.AddFallKick(2f);

            _player.StateChart.Call("send_event", "onGrounded");
        }

        _player.CurrentFallVelocity = _player.Velocity.Y;
    }
}