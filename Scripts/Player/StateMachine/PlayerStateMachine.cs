using Godot;

public partial class PlayerStateMachine : Node
{
    [Export] private bool _debug = false;
    [ExportCategory("References")]
    [Export] protected PlayerController _player;

    public override void _Ready()
    {
        Node stateMachine = GetNodeOrNull("%StateMachine");

        if (stateMachine is PlayerStateMachine)
            _player = GetOwner<PlayerController>();
    }

    public override void _Process(double delta)
    {
        if (_player != null)
        {
            _player.StateChart.Call("set_expression_property", "Velocity", _player.Velocity);
            _player.StateChart.Call("set_expression_property", "Speed", _player.Speed);
            _player.StateChart.Call("set_expression_property", "Player Hitting Head", _player.CrouchCheck.IsColliding());
            _player.StateChart.Call("set_expression_property", "Looking At:", _player.InteractionRaycast.GetCollider());
            _player.StateChart.Call("set_expression_property", "Dash Cooldown:", _player.DashCooldownTimer);
            _player.StateChart.Call("set_expression_property", "Step Handler", _player.StepHandler.StepStatus);
        }
    }
}