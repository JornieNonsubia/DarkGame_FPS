using Godot;
using System;

public partial class GameManager : Node
{
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventAction action)
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