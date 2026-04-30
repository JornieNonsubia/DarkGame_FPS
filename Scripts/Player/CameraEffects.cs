using Godot;

public partial class CameraEffects : Camera3D
{
    [ExportCategory("References")]
    [Export] public PlayerController Player;

    [ExportCategory("Effects")]
    [Export] public bool EnableTilt = true;
    [Export] public bool EnableFallKick = true;
    [Export] public bool EnableDamageKick = true;
    [Export] public bool EnableWeaponKick = true;
    [Export] public bool EnableScreenShake = true;
    [Export] public bool EnableHeadBob = true;

    [ExportCategory("Kick & Recoil Settings")]
    [ExportGroup("Run Tilt")]
    [Export] public float RunPitch = 0.05f; // Degrees
    [Export] public float RunRoll = 0.12f; // Degrees
    [Export] public float MaxPitch = 1f; // Degrees
    [Export] public float MaxRoll = 2.5f; // Degrees
    [ExportGroup("Camera Kick")]
    [ExportSubgroup("Fall Kick")]
    [Export] public float FallTime = 0.3f; // in secs
    [ExportSubgroup("Damage Kick")]
    [Export] public float DamageTime = 0.3f; // in secs
    [ExportSubgroup("Weapon Kick")]
    [Export] public float WeaponDecay = 1f;
    [ExportSubgroup("Headbob")]
    [Export(PropertyHint.Range, "0.0,0.1,0.001")]
    private float _bobPitch = 0.05f;
    [Export(PropertyHint.Range, "0.0,0.1,0.001")]
    private float _bobRoll = 0.025f;
    [Export(PropertyHint.Range, "0.0,0.04,0.001")]
    private float _bobUp = 0.005f;
    [Export(PropertyHint.Range, "3.0,8.1,0.1")]
    private float _bobFrequency = 6f;


    // Fall Kick vars
    private float _fallValue = 0;
    private float _fallTimer = 0;

    // Damage Kick vars
    private float _damagePitch = 0f;
    private float _damageRoll = 0f;
    private float _damageTimer = 0f;

    // Weapon Kick vars
    Vector3 _weaponKickAngles = Vector3.Zero;

    // Screen Shake vars
    private Tween _screenShakeTween;
    private const float MinScreenShake = 0.05f;
    private const float MaxScreenShake = 0.5f;

    private float _stepTimer = 0f;

    public override void _Process(double delta)
    {
        CalculateViewOffset(delta);

        if (Input.IsActionJustPressed("LMB"))
            AddWeaponKick(4f, 2f, 2f);
        
    }

    void CalculateViewOffset(double delta)
    {
        float fDelta = (float)delta;
        if (Player == null) return;

        _fallTimer -= fDelta;

        Vector3 vel = Player.Velocity;

        float speed = new Vector2(vel.X, vel.Z).Length();
        if (speed > 0.1f && Player.IsOnFloor())
        {
            _stepTimer += fDelta * (speed / _bobFrequency);
            _stepTimer %= 1f;
        }
        else
            _stepTimer = 0f;
        double bobSin = Mathf.Sin(_stepTimer * 2f * Mathf.Pi) * 0.5; // 0.5 reduces the magnitude of the sine wave, i.e. less movement
            
        

        Vector3 angles = Vector3.Zero;
        Vector3 offset = Vector3.Zero;

        // Camera Tilt
        if (EnableTilt)
        {
            Vector3 forward = GlobalTransform.Basis.Z;
            Vector3 right = GlobalTransform.Basis.X;

            float forwardDot = vel.Dot(forward);
            float forwardTilt = Mathf.Clamp(
                forwardDot * Mathf.DegToRad(RunPitch),
                Mathf.DegToRad(-MaxPitch),
                Mathf.DegToRad(MaxPitch)
            );
            angles.X += forwardTilt;

            float rightDot = vel.Dot(right);
            float sideTilt = Mathf.Clamp(
                rightDot * Mathf.DegToRad(RunRoll),
                Mathf.DegToRad(-MaxRoll),
                Mathf.DegToRad(MaxRoll)
            );
            angles.Z -= sideTilt;
        }

        // Fall Kick
        if (EnableFallKick)
        {
            double fallRation = Mathf.Max(0.0, _fallTimer / FallTime);
            float fallKickAmount = (float)fallRation * _fallValue;
            angles.X -= fallKickAmount;
            offset.Y -= fallKickAmount;
        }

        // Damage Kick
        if (EnableDamageKick)
        {
            double damageRatio = Mathf.Max(0f, _damageTimer);
            // damageRatio = Mathf.Ease(damageRatio, -2); // for ease over time
            angles.X += (float)damageRatio * _damagePitch;
            angles.Z += (float)damageRatio * _damageRoll;
        }

        // Weapon Kick
        if (EnableWeaponKick)
        {
            _weaponKickAngles = _weaponKickAngles.MoveToward(Vector3.Zero, WeaponDecay * fDelta);
            angles += _weaponKickAngles;
        }

        // HeadBob
        if (EnableHeadBob)
        {
            float pitchDelta = (float)bobSin * Mathf.DegToRad(_bobPitch) * speed;
            angles.X -= pitchDelta;

            float rollDelta = (float)bobSin * Mathf.DegToRad(_bobRoll) * speed;
            angles.Z -= rollDelta;
            
            float bobHeight = (float)bobSin * speed * _bobUp;
            offset.Y += bobHeight;
        }
        
        Position = offset;
        Rotation = angles;
    }

    public void AddFallKick(float fallStrength)
    {
        _fallValue = Mathf.DegToRad(fallStrength);
        _fallTimer = FallTime;
    }

    public void AddDamageKick(float pitch, float roll, Vector3 source)
    {
        Vector3 forward = GlobalTransform.Basis.Z;
        Vector3 right = GlobalTransform.Basis.X;
        Vector3 direction = GlobalTransform.Origin.DirectionTo(source);
        float forwardDot = direction.Dot(forward);
        float rightDot = direction.Dot(right);
        _damagePitch = Mathf.DegToRad(pitch) * forwardDot;
        _damageRoll = Mathf.DegToRad(roll) * rightDot;
        _damageTimer = DamageTime;
    }

    public void AddWeaponKick(float pitch, float yaw, float roll)
    {
        _weaponKickAngles.X += Mathf.DegToRad(pitch);
        _weaponKickAngles.Y += Mathf.DegToRad((float)GD.RandRange(-yaw, yaw));
        _weaponKickAngles.Z += Mathf.DegToRad((float)GD.RandRange(-roll, roll));
    }

    public void AddScreenShake(float amount, float seconds)
    {
        if (_screenShakeTween != null && _screenShakeTween.IsValid())
            _screenShakeTween.Kill();

        _screenShakeTween = CreateTween();
        _screenShakeTween.TweenMethod(Callable.From<float>((alpha) => UpdateScreenShake(alpha, amount)), 0f, 1f, seconds)
            .SetEase(Tween.EaseType.Out);
        _screenShakeTween.Finished += () =>
        {
            HOffset = 0f;
            VOffset = 0f;
        };
    }

    private void UpdateScreenShake(float alpha, float amount)
    {
        amount = Mathf.Remap(amount, 0f, 1f, MinScreenShake, MaxScreenShake);
        float currentShakeAmount = amount * (1f - alpha);
        HOffset = (float)GD.RandRange(-currentShakeAmount, currentShakeAmount);
        VOffset = (float)GD.RandRange(-currentShakeAmount, currentShakeAmount);
    }
}