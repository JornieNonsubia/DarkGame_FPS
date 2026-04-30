using Godot;

public partial class GameManager : Node
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey action)
        {
            if (action.IsActionPressed(("dev_exit")))
                GetTree().Quit();

            if (action.IsActionPressed("dev_reload"))
            {
                GetTree().ReloadCurrentScene();
            }
        }
    }
}