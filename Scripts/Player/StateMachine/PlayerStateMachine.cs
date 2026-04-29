using Godot;

public partial class PlayerStateMachine : Node
{
    [Export] private bool _debug = false;
    [ExportCategory("References")]
    [Export]protected PlayerController PlayerController;

    public override void _Ready()
    {
        Node stateMachine = GetNodeOrNull("%StateMachine");

        if (stateMachine is PlayerStateMachine playerStateMachine)
            PlayerController = GetOwner<PlayerController>();
    }

    public override void _Process(double delta)
    {
        if (PlayerController != null)
        {
            PlayerController.stateChart.Call("set_expression_property", "Player Velocity", PlayerController.Velocity);
            PlayerController.stateChart.Call("set_expression_property", "Player Hitting Head", PlayerController.CrouchCheck.IsColliding());
            PlayerController.stateChart.Call("set_expression_property", "Looking At:", PlayerController.interectionRaycast.GetCollider()); 
        }
    }
}