using Godot;

public partial class CameraController : Node3D
{
    [Export] private bool _debug = false;

    [ExportCategory("References")]
    [Export] private PlayerController _player;
    [Export] private MouseCaptureComponent _componentMouseCapture;

    [ExportCategory("Camera Settings")]
    [ExportGroup("Camera Tilt")]
    [Export(PropertyHint.Range, "-90,60")] private int _tiltLowerLimit = -90;
    [Export(PropertyHint.Range, "60,90")] private int _tiltUpperLimit = 90;
    [ExportGroup("Crouch Vertical Movement")]
    [Export] private float _crouchOffset = 0f;
    [Export] private float _crouchingStateChangeSpeed = 3f;
    [ExportGroup("Step Smoothing")]
    [Export] private float _stepSpeed = 8f;

    private float _targetHeight;
    private bool _stepSmoothing = false;

    private float _offsetHeight;

    private Vector3 _rotation = Vector3.Zero;

    private const float DefaultHeight = 0.5f;

    public override void _Ready()
    {
        _rotation = _player.Rotation;
        _offsetHeight = DefaultHeight;
    }

    public override void _Process(double delta)
    {
        float fDelta = (float)delta;
        UpdateCameraRotation(_componentMouseCapture.MouseInput, fDelta);

        if (_stepSmoothing)
        {
            _targetHeight = Mathf.Lerp(_targetHeight, 0f, _stepSpeed * fDelta);

            if (Mathf.Abs(_targetHeight) < 0.01)
            {
                _targetHeight = 0;
                _stepSmoothing = false;
            }
            Vector3 pos = Position;
            pos.Y = _offsetHeight + _targetHeight;
            Position = pos;
        }
    }

    private void UpdateCameraRotation(Vector2 input, float fDelta)
    {
        _rotation.X += input.Y * fDelta;
        _rotation.Y += input.X * fDelta;
        _rotation.X = Mathf.Clamp(_rotation.X, Mathf.DegToRad(_tiltLowerLimit), Mathf.DegToRad(_tiltUpperLimit));

        Vector3 playerRotation = new Vector3(0f, _rotation.Y, 0f);
        Vector3 cameraRotation = new Vector3(_rotation.X, 0f, 0f);

        Basis = Basis.FromEuler(cameraRotation);
        _player.UpdateRotation(playerRotation);
    }

    public void UpdateCameraHeight(double delta, int direction)
    {
        float fDelta = (float)delta;

        if (_offsetHeight >= _crouchOffset && _offsetHeight <= DefaultHeight)
            _offsetHeight = Mathf.Clamp(_offsetHeight + (_crouchingStateChangeSpeed * direction) * fDelta, _crouchOffset, DefaultHeight);

        if (Position.Y >= _crouchOffset && Position.Y <= DefaultHeight)
        {
            Vector3 pos = Position;
            pos.Y = Mathf.Clamp(pos.Y + (_crouchingStateChangeSpeed * direction) * fDelta, _crouchOffset, DefaultHeight);
            Position = pos;
        }
    }

    public void SmoothStep(float height)
    {
        _targetHeight -= height;
        _stepSmoothing = true;
    }
}