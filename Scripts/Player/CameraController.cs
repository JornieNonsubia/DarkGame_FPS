using Godot;
using System;
using Range = Godot.Range;

public partial class CameraController : Node3D
{
    [Export] private bool _debug = false;

    [ExportCategory("References")]
    [Export] private PlayerController _playerController;
    [Export] private MouseCaptureComponent _componentMouseCapture;

    [ExportCategory("Camera Settings")]
    [ExportGroup("Camera Tilt")]
    [Export(PropertyHint.Range, "-90,60")] private int _tiltLowerLimit = -90;
    [Export(PropertyHint.Range, "60,90")] private int _tiltUpperLimit = 90;
    [ExportGroup("Crouch Vertical Movement")]
    [Export] private float _crouchOffset = 0f;
    [Export] private float _crouchingStateChangeSpeed = 3f;

    private Vector3 _rotation = Vector3.Zero;

    private const float DefaultHeight = 0.5f;

    public override void _Process(double delta)
    {
        UpdateCameraRotation(_componentMouseCapture.MouseInput);
    }

    private void UpdateCameraRotation(Vector2 input)
    {
        _rotation.X += input.Y;
        _rotation.Y += input.X;
        _rotation.X = Mathf.Clamp(_rotation.X, Mathf.DegToRad(_tiltLowerLimit), Mathf.DegToRad(_tiltUpperLimit));

        Vector3 playerRotation = new Vector3(0f, _rotation.Y, 0f);
        Vector3 cameraRotation = new Vector3(_rotation.X, 0f, 0f);

        Basis = Basis.FromEuler(cameraRotation, EulerOrder.Yxz);
        _playerController.UpdateRotation(playerRotation);
    }

    public void UpdateCameraHeight(double delta, int direction)
    {
        if (Position.Y >= _crouchOffset && Position.Y <= DefaultHeight)
        {
            Vector3 pos = Position;
            pos.Y = Mathf.Clamp(pos.Y + (_crouchingStateChangeSpeed * direction) * (float)delta, _crouchOffset, DefaultHeight);
            Position = pos;
        }
    }
}