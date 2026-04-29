using Godot;
using GodotStateCharts;

public partial class PlayerController : CharacterBody3D
{
    [ExportCategory("References")]
    [Export] public CameraController Head;
    [Export] public Node stateChart;
    [Export] public ShapeCast3D CrouchCheck;
    [Export] public RayCast3D interectionRaycast;
    [Export] protected CollisionShape3D _standingCollision;
    [Export] protected CollisionShape3D _crouchingCollision;

    [ExportCategory("Movement")]
    [ExportGroup("Easing")]
    [Export] private float _acceleration = 10f;
    [Export] private float _deceleration = 25f;
    [ExportGroup("Speed")]
    [Export] private float _defaultSpeed = 7f;
    [Export(PropertyHint.Range, "100,200,1")] private float _sprintSpeedPercent = 130f; // 100% is base speed
    [Export(PropertyHint.Range, "0,100,1")] private float _crouchSpeedPercent = 40f;
    [ExportCategory("Dash Settings")]
    [Export] private float dashVelocity = 80f;
    [Export] private float dashCooldown = 2f;

    public Vector2 InputDir = Vector2.Zero;
    private Vector3 _moveVelocity = Vector3.Zero;
    private float sprintModifier = 1;
    private float crouchModifier = 1;
    private float speed = 0;


    public override void _PhysicsProcess(double delta)
    {
        float fDelta = (float)delta;
        if (!IsOnFloor())
            Velocity += GetGravity() * fDelta;

        // Crouch has
        speed = _defaultSpeed * crouchModifier * sprintModifier;

        InputDir = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");

        Vector2 currentVelocity2D = new Vector2(Velocity.X, Velocity.Z);

        Vector3 direction = (Transform.Basis * new Vector3(InputDir.X, 0, InputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            Vector2 targetVelocity = new Vector2(direction.X, direction.Z) * speed;
            currentVelocity2D = currentVelocity2D.Lerp(targetVelocity, _acceleration * fDelta);
        }
        else
        {
            currentVelocity2D = currentVelocity2D.MoveToward(Vector2.Zero, _deceleration * fDelta);
        }

        _moveVelocity = new Vector3(currentVelocity2D.X, Velocity.Y, currentVelocity2D.Y);
        Velocity = _moveVelocity;

        MoveAndSlide();
    }

    public void UpdateRotation(Vector3 rotationInput)
    {
        Transform3D gt = GlobalTransform;
        gt.Basis = Basis.FromEuler(rotationInput);
        GlobalTransform = gt;
    }

    // ===== Methods for States =====

    public void Sprint()
    {
        // Making Sprint modifier a %
        sprintModifier = _sprintSpeedPercent / 100;
    }

    public void Walk()
    {
        // Setting speed to 100%
        sprintModifier = 1;
    }

    public void Stand()
    {
        // Setting speed to 100%
        crouchModifier = 1;
        _crouchingCollision.Disabled = true;
        _standingCollision.Disabled = false;
    }

    public void Crouch()
    {
        // Adding % speed reduction 
        crouchModifier = _crouchSpeedPercent / 100;
        _standingCollision.Disabled = true;
        _crouchingCollision.Disabled = false;
    }

    public void Dash()
    {
        Vector3 dashDirection = Vector3.Zero;

        if (InputDir != Vector2.Zero)
        {
            dashDirection = (Transform.Basis * new Vector3(InputDir.X, 0, InputDir.Y)).Normalized();
        }

        if (dashDirection == Vector3.Zero)
        {
            Vector3 forward = -GlobalTransform.Basis.Z;
            dashDirection = new Vector3(forward.X, 0, forward.Z).Normalized();

            if (dashDirection == Vector3.Zero)
                dashDirection = -Transform.Basis.Z;
        }

        Velocity = new Vector3(dashDirection.X * dashVelocity, 0.2f, dashDirection.Z * dashVelocity
        );
    }
}


// ========  OLD STUFF ========
// [ExportGroup("Movement Settings")]
// [Export] public float WalkSpeed = 5.0f;
// [Export] public float SprintModifier = 8.0f;
// [Export] public float CrouchModifier = 2.5f;
// [Export] public float Dash = 15.0f;
// [Export] public float Acceleration = 10.0f;
// [Export] public float Friction = 12.0f;
//
// [ExportGroup("Camera & Effects")]
// [Export] public float CameraTiltAmount = 0.05f;
// [Export] public float CameraTiltSpeed = 5.0f;
// [Export] public float MouseSensitivity = 0.002f;
// [Export] public float MinPitch = -80f;
// [Export] public float MaxPitch = 89f;
//
//
// [ExportGroup("Crouch Settings")]
// [Export] public float CrouchHeight = 1.2f;
// [Export] public float StandingHeight = 2.0f;
// [Export] public float CrouchTransitionSpeed = 0.2f;
//
// [ExportGroup("Nodes")]
// [Export] public Node3D Head;
// [Export] public Camera3D Camera;
// [Export] public CollisionShape3D CollisionShape;
// [Export] public Node StateChartNode; // Прив'яжи сюди StateChart
//
// private StateChart _stateChart;
// private Vector3 _direction = Vector3.Zero;
// private float _currentSpeed = 0.0f;
// private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
// private Tween _crouchTween;
// private float _rotationX = 0f;
//
//
// public override void _Ready()
// {
//     Input.MouseMode = Input.MouseModeEnum.Captured;
//
//        _stateChart = GetNode<StateChart>("StateChart");
// }
//
// public override void _PhysicsProcess(double delta)
// {
//     float d = (float)delta;
//     Vector3 velocity = Velocity;
//
//     // Гравітація
//     if (!IsOnFloor())
//     {
//         velocity.Y -= _gravity * d;
//     }
//
//     // Ввід (Згідно твого Input Map)
//     Vector2 inputDir = Input.GetVector("StrafeLeft", "StrafeRight", "MoveForward", "MoveBackward");
//     _direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
//
//     // Логіка станів (відправляємо івенти згідно твоїх назв)
//     HandleMovementState();
//
//     
//     // Фізика руху
//     if (_direction != Vector3.Zero)
//     {
//         velocity.X = Mathf.Lerp(velocity.X, _direction.X * _currentSpeed, Acceleration * d);
//         velocity.Z = Mathf.Lerp(velocity.Z, _direction.Z * _currentSpeed, Acceleration * d);
//     }
//     else
//     {
//         velocity.X = Mathf.Lerp(velocity.X, 0, Friction * d);
//         velocity.Z = Mathf.Lerp(velocity.Z, 0, Friction * d);
//     }
//
//     HandleCameraTilt(inputDir.X, d);
//
//     Velocity = velocity;
//     MoveAndSlide();
// }
//
// public override void _Input(InputEvent @event)
// {
//     if (@event is InputEventMouseMotion mouseMotion)
//     {
//
//         // RotateY(-mouseMotion.Relative.X * MouseSensitivity);
//         // _rotationX = mouseMotion.Relative.Y * MouseSensitivity;
//         // //rotationX = Mathf.Clamp(rotationX, Mathf.DegToRad(-120f),Mathf.DegToRad(-120f));
//         // //Head.Rotation = new Vector3(rotationX, Head.Rotation.Y, Head.Rotation.Z);
//         // Vector3 headRotation = Head.Rotation;
//         // headRotation.X = _rotationX;
//         // Head.Rotation = headRotation;
//         
//         
//         RotateY(-mouseMotion.Relative.X * MouseSensitivity);
//
//         _rotationX -= mouseMotion.Relative.Y * MouseSensitivity;
//         
//         _rotationX = Mathf.Clamp(_rotationX, Mathf.DegToRad(MinPitch), Mathf.DegToRad(MaxPitch));
//         
//         Vector3 headRotation = Head.Rotation;
//         headRotation.X = _rotationX;
//         Head.Rotation = headRotation;
//     }
// }
//
// private void HandleMovementState()
// {
//     if (_stateChart == null) return;
//
//     if (IsOnFloor())
//     {
//         // Пріоритет 1: Ривок
//         if (Input.IsActionJustPressed("Dash"))
//         {
//             _stateChart.SendEvent("ToDashing");
//             return;
//         }
//
//         // Пріоритет 2: Присідання
//         if (Input.IsActionPressed("Crouch"))
//         {
//             // Якщо ми рухаємось чи стоїмо - переходимо в Crouch
//             _stateChart.SendEvent("ToCrouching");
//         }
//         // Пріоритет 3: Рух
//         else if (_direction != Vector3.Zero)
//         {
//             if (Input.IsActionPressed("Sprint"))
//                 _stateChart.SendEvent("ToSprinting");
//             else
//                 _stateChart.SendEvent("ToWalking");
//         }
//         // Пріоритет 4: Спокій
//         else
//         {
//             // Тобі треба додати івенти ToIdle в Walking, Sprinting та Crouching
//             _stateChart.SendEvent("ToIdle");
//         }
//     }
// }
//
// private void HandleCameraTilt(float inputX, float delta)
// {
//     float targetTilt = -inputX * CameraTiltAmount; 
//     Vector3 currentRot = Head.Rotation;
//     currentRot.Z = Mathf.Lerp(currentRot.Z, targetTilt, CameraTiltSpeed * delta);
//     Head.Rotation = currentRot;
// }
//
// // --- СИГНАЛИ (Підключи їх у редакторі StateChart) ---
// // Увага: назви методів тепер відповідають твоїм станам
//
// public void OnIdleEntered() => _currentSpeed = 0.0f;
//
// public void OnWalkingEntered() => _currentSpeed = WalkSpeed;
//
// public void OnSprintingEntered() => _currentSpeed = SprintModifier;
//
// public void OnDashingEntered() => _currentSpeed = Dash;
//
// public void OnCrouchingEntered()
// {
//     _currentSpeed = CrouchModifier;
//     DoCrouch(true);
// }
//
// public void OnCrouchingExited()
// {
//     DoCrouch(false);
// }
//
// private void DoCrouch(bool isCrouching)
// {
//     if (_crouchTween != null && _crouchTween.IsRunning())
//         _crouchTween.Kill();
//
//     _crouchTween = CreateTween();
//     _crouchTween.SetParallel(true);
//
//     float targetHeight = isCrouching ? CrouchHeight : StandingHeight;
//     float targetHeadY = isCrouching ? CrouchHeight - 0.2f : StandingHeight - 0.2f; 
//     
//     // Змінюємо shape:height для CapsuleShape3D
//     _crouchTween.TweenProperty(CollisionShape.Shape, "height", targetHeight, CrouchTransitionSpeed);
//     _crouchTween.TweenProperty(CollisionShape, "position:y", targetHeight / 2, CrouchTransitionSpeed);
//     _crouchTween.TweenProperty(Head, "position:y", targetHeadY, CrouchTransitionSpeed);
// }