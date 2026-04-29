using System.Diagnostics;
using Godot;

public partial class MouseCaptureComponent : Node
{
    [Export] private bool _debug = false;

    [ExportCategory("Mouse Capture Settings")]
    [Export] private Input.MouseModeEnum _currentMouseMode = Input.MouseModeEnum.Captured;
    [Export] private float _mouseSensitivityModifier = 0.002f;
    [Export(PropertyHint.Range, "1,200,1")] private float _mouseSensitivity = 50f;

    public Vector2 MouseInput;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            // Reducing overall mouse sens and additionally reducing it by user's percent amount 
            MouseInput.X += -mouseMotion.ScreenRelative.X * (_mouseSensitivityModifier/100) * _mouseSensitivity;
            MouseInput.Y += -mouseMotion.ScreenRelative.Y * (_mouseSensitivityModifier/100) * _mouseSensitivity;
        }

        if (_debug)
        {
            GD.Print(MouseInput);
        }
    }

    public override void _Ready()
    {
        Input.MouseMode = _currentMouseMode;
    }

    public override void _Process(double delta)
    {
        MouseInput = Vector2.Zero;
    }
}