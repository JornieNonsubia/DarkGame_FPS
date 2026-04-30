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
            PlayerController.StateChart.Call("set_expression_property", "Velocity", PlayerController.Velocity);
            PlayerController.StateChart.Call("set_expression_property", "Speed", PlayerController.Speed);
            PlayerController.StateChart.Call("set_expression_property", "Player Hitting Head", PlayerController.CrouchCheck.IsColliding());
            PlayerController.StateChart.Call("set_expression_property", "Looking At:", PlayerController.InteractionRaycast.GetCollider()); 
            PlayerController.StateChart.Call("set_expression_property", "Dash Cooldown:", PlayerController.DashCooldownTimer); 
            
        }
    }
}