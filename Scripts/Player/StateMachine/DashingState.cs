using Godot;

public partial class DashingState : PlayerStateMachine
{
    [Export] private float _dashDuration = 0.2f; // Тривалість ривка в секундах
    private float _dashTimer = 0f;

    void OnDashingStateEntered()
    {
        _player.Dash();
        _dashTimer = _dashDuration;
    }

    private void OnDashingStatePhysicsProcessing(double delta)
    {
        if (_dashTimer > 0)
        {
            _dashTimer -= (float)delta;

            if (_dashTimer <= 0)
                _player.StateChart.Call("send_event", "onWalking");
        }
    }

    private void OnDashingStateExited()
    {
        Vector3 currentVel = _player.Velocity;
        _player.Velocity = new Vector3(currentVel.X * 0.2f, currentVel.Y, currentVel.Z * 0.2f);
    }
}