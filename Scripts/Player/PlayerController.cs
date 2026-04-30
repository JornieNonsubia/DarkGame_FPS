using Godot;
using GodotStateCharts;

public partial class PlayerController : CharacterBody3D
{
    [ExportCategory("References")]
    [Export] public CameraController Head;
    [Export] public CameraEffects CameraEffects;
    [Export] public ShapeCast3D CrouchCheck;
    [Export] public RayCast3D InteractionRaycast;
    [Export] public CollisionShape3D StandingCollision;
    [Export] public CollisionShape3D CrouchingCollision;
    [Export] public StepHandlerComponent StepHandler;
    [Export] public Node StateChart;

    [ExportCategory("Movement")]
    [Export] public float FallVelocityThreshold = -5f;
    [ExportGroup("Easing")]
    [Export] private float _acceleration = 10f;
    [Export] private float _deceleration = 25f;
    [ExportGroup("Speed")]
    [Export] private float _defaultSpeed = 7f;
    [Export(PropertyHint.Range, "100,200,1")] private float _sprintSpeedPercent = 130f; // 100% is base speed
    [Export(PropertyHint.Range, "0,100,1")] private float _crouchSpeedPercent = 40f;
    [ExportCategory("Dash Settings")]
    [Export] private float _dashVelocity = 40f;
    [Export] private float _dashCooldown = 5f; // in secs
    [ExportCategory("DataHelpers")]
    [Export] public Vector3 DataRelativeVelocity;


    private float _currentFallVelocity;
    private Vector3 _previousVelocity;
    private Vector2 _inputDir = Vector2.Zero;

    // Movements
    private Vector3 _moveVelocity = Vector3.Zero;
    private float _sprintModifier = 1;
    private float _crouchModifier = 1;
    private float _speed = 0;

    // Dash
    private float _dashCooldownTimer = 0f;
    private bool _canDash = true;

    public bool CanDash { get => _canDash; }
    public float DashCooldownTimer { get => _dashCooldownTimer; }
    public float Speed { get => _speed; }
    public float CurrentFallVelocity { get => _currentFallVelocity; set => _currentFallVelocity = value; }
    public Vector3 PreviousVelocity { get => _previousVelocity; }
    public Vector2 InputDir { get => _inputDir; }



    public override void _PhysicsProcess(double delta)
    {
        float fDelta = (float)delta;
        if (_dashCooldownTimer > 0)
        {
            _dashCooldownTimer -= fDelta;
            if (_dashCooldownTimer <= 0)
                _canDash = true;
        }

        if (IsOnFloor())
            StepHandler.HandleStepClimbing();

        if (!IsOnFloor())
            Velocity += GetGravity() * fDelta;

        _previousVelocity = Velocity;

        // Modifying our speed with modifiers
        _speed = _defaultSpeed * _crouchModifier * _sprintModifier;

        _inputDir = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");

        Vector2 currentVelocity2D = new Vector2(Velocity.X, Velocity.Z);

        Vector3 direction = (Transform.Basis * new Vector3(_inputDir.X, 0, _inputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            Vector2 targetVelocity = new Vector2(direction.X, direction.Z) * _speed;
            currentVelocity2D = currentVelocity2D.Lerp(targetVelocity, _acceleration * fDelta);
        }
        else
        {
            currentVelocity2D = currentVelocity2D.MoveToward(Vector2.Zero, _deceleration * fDelta);
        }

        _moveVelocity = new Vector3(currentVelocity2D.X, Velocity.Y, currentVelocity2D.Y);
        Velocity = _moveVelocity;

        MoveAndSlide();
        StepHandler.HandleStepClimbing();
    }

    public void UpdateRotation(Vector3 rotationInput)
    {
        Transform3D gt = GlobalTransform;
        gt.Basis = Basis.FromEuler(rotationInput);
        GlobalTransform = gt;
    }

    public bool CheckFallSpeed()
    {
        if (_currentFallVelocity < FallVelocityThreshold)
        {
            _currentFallVelocity = 0f;
            return true;
        }
        else
        {
            _currentFallVelocity = 0f;
            return false;
        }
    }

    // ===== Methods for States =====

    public void Sprint()
    {
        // Making Sprint modifier a %
        _sprintModifier = _sprintSpeedPercent / 100;
    }

    public void Walk()
    {
        // Setting speed to 100%
        _sprintModifier = 1;
    }

    public void Stand()
    {
        // Setting speed to 100%
        _crouchModifier = 1;
        CrouchingCollision.Disabled = true;
        StandingCollision.Disabled = false;
    }

    public void Crouch()
    {
        // Adding % speed reduction 
        _crouchModifier = _crouchSpeedPercent / 100;
        StandingCollision.Disabled = true;
        CrouchingCollision.Disabled = false;
    }

    public void Dash()
    {
        _dashCooldownTimer = _dashCooldown;
        _canDash = false;
        Vector3 dashDirection = Vector3.Zero;

        if (_inputDir != Vector2.Zero)
        {
            dashDirection = (Transform.Basis * new Vector3(_inputDir.X, 0, _inputDir.Y)).Normalized();
        }

        if (dashDirection == Vector3.Zero)
        {
            Vector3 forward = -GlobalTransform.Basis.Z;
            dashDirection = new Vector3(forward.X, 0, forward.Z).Normalized();

            if (dashDirection == Vector3.Zero)
                dashDirection = -Transform.Basis.Z;
        }

        Velocity = new Vector3(
            dashDirection.X * _dashVelocity,
            0.0f,
            dashDirection.Z * _dashVelocity
        );
    }
}