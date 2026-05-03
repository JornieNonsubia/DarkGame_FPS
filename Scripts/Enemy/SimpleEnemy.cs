using Godot;

public partial class SimpleEnemy : BaseEnemy
{
    [Export] private float _movementSpeed = 3f;
    [Export] private float _acceleration = 3f;
    [Export] private float _deceleration = 1f;
    [Export] private float _meleeRange = 1f;

    private NavigationAgent3D _navAgent;
    private Node _stateChart;
    private HealthComponent _healthComponent;
    private AnimationPlayer _animPlayer;
    private AnimationTree _animTree;

    private Node3D _target;
    private AnimationNodeStateMachinePlayback _animTreeState;


    public override void _Ready()
    {
        _navAgent = GetNodeOrNull<NavigationAgent3D>("NavigationAgent3D");
        _stateChart = GetNodeOrNull("StateChart");
        _healthComponent = GetNodeOrNull<HealthComponent>("HealthComponent");
        _animPlayer = GetNodeOrNull<AnimationPlayer>("Spider_enemy/AnimationPlayer");
        _animTree = GetNodeOrNull<AnimationTree>("AnimationTree");

        Callable.From(base._Ready).CallDeferred();

        _target = (Node3D)GetTree().GetFirstNodeInGroup("Players");

        _healthComponent.Died += OnDied;
        _navAgent.VelocityComputed += OnVelocityComputed;

        _animTreeState = (AnimationNodeStateMachinePlayback)_animTree.Get("parameters/playback");
    }

    public override void _PhysicsProcess(double delta)
    {
        float fDelta = (float)delta;

        if (!IsOnFloor())
        {
            Vector3 vel = Velocity;
            vel.Y -= 20f * fDelta;
            Velocity = vel;
        }

        MoveAndSlide();
        UpdateBlends();
    }


    // ===== Custom ===== 

    private void UpdateBlends()
    {
        float moveAmount = Velocity.Length();
        moveAmount = Mathf.Remap(moveAmount, 0, _movementSpeed, 0, 1);
        _animTree.Set("parameters/Chasing/IdleChaseBlend/blend_position", moveAmount);
    }

    private async void Attack()
    {
        Velocity = Vector3.Zero;
        _navAgent.Velocity = Vector3.Zero;

        if (_target == null) return;

        // Rotating to face Target
        Vector3 dir = (_target.GlobalPosition - GlobalPosition).Normalized();
        float targetRotation = Mathf.Atan2(dir.X, dir.Z);
        Vector3 rot = Rotation;
        rot.Y = targetRotation;
        Rotation = rot;

        // Restarting animation if 
        if (_animTreeState.GetCurrentNode() != "Attack")
            _animTreeState.Travel("Attack");
        else
            _animTreeState.Start("Attack");

        await ToSignal(_animTree, AnimationTree.SignalName.AnimationFinished);

        float distance = GlobalPosition.DistanceTo(_target.GlobalPosition);

        if (distance <= _meleeRange)
            Attack();
        else
            _stateChart.Call("send_event", "toChasing");
    }


    // ===== Signals =====

    private void OnChasingStatePhysicsProcessing(double delta)
    {
        float fDelta = (float)delta;

        if (_target == null)
            return;

        _navAgent.SetTargetPosition(_target.GlobalPosition);

        float distance = GlobalPosition.DistanceTo(_target.GlobalPosition);

        if (distance <= _meleeRange)
        {
            _stateChart.Call("send_event", "toAttack");
            return;
        }

        if (_navAgent.IsNavigationFinished())
        {
            _navAgent.Velocity = Vector3.Zero;

            if (_animPlayer != null && _animPlayer.CurrentAnimation != "Idle")
                _animPlayer.Play("Idle");

            return;
        }

        Vector3 nextPos = _navAgent.GetNextPathPosition();
        Vector3 dir = (nextPos - GlobalPosition).Normalized();

        _navAgent.Velocity = dir * _movementSpeed;

        if (!_navAgent.AvoidanceEnabled)
        {
            Vector3 vel = Velocity;
            float targetVelX = dir.X * _movementSpeed;
            float targetVelZ = dir.Z * _movementSpeed;

            vel.X = Mathf.MoveToward(vel.X, targetVelX, _acceleration * fDelta);
            vel.Z = Mathf.MoveToward(vel.Z, targetVelZ, _deceleration * fDelta);
            Velocity = vel;
        }

        if (dir.Length() > 0.01)
        {
            float targetRotation = Mathf.Atan2(dir.X, dir.Z);

            Vector3 rot = Rotation;
            rot.Y = Mathf.LerpAngle(Rotation.Y, targetRotation, 5f * fDelta);
            Rotation = rot;
        }
    }

    private void OnDetectionAreaBodyEntered(Node3D body)
    {
        if (body.IsInGroup("Players"))
        {
            OnTriggered();
        }
    }

    private void OnChasingStateEntered()
    {
        if (_animTreeState.GetCurrentNode() != "Chasing")
            _animTreeState.Travel("Chasing");
    }

    private void OnAttackStateEntered()
    {
        Attack();
    }

    private void OnVelocityComputed(Vector3 safeVelocity)
    {
        Vector3 vel = Velocity;

        Vector3 targetVel = new Vector3(safeVelocity.X, Velocity.Y, safeVelocity.Z);
        float accel = (safeVelocity.Length() > 0.01) ? _acceleration : _deceleration;
        vel.X = (float)Mathf.MoveToward(vel.X, targetVel.X, accel * GetPhysicsProcessDeltaTime());
        vel.Z = (float)Mathf.MoveToward(vel.Z, targetVel.Z, accel * GetPhysicsProcessDeltaTime());
    }

    // ===== Наслідуєме =====

    public override void OnTriggered()
    {
        _stateChart.Call("send_event", "toChasing");
    }

    private void OnDied()
    {
        QueueFree();
    }
}