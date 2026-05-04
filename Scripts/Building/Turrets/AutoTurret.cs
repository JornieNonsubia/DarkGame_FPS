using Godot;

public partial class AutoTurret : RigidBody3D
{
    [Export] private Node3D _head;
    [Export] private Node3D _neck;
    [Export] private float _rotationSpeed = 5f;


    private Node3D[] _targets = [];
    private Node3D target;

    public override void _PhysicsProcess(double delta)
    {
        // Node3D closestTarget = null;
        // Node3D furthestTarget = null;
        //
        // foreach (Node3D target in _targets)
        //     GD.Print(target.ToString());
        //
        // if (_targets.Length > 0)
        // {
        //     GD.Print("_Process start");
        //
        //
        //     foreach (Node3D target in _targets)
        //     {
        //         closestTarget = target;
        //     }
        //
        // }

        if (target != null)
        {
            LookAtTarget(target);
        }


    }

    private void OnArea3DBodyEntered(Node3D body)
    {
        target = body;
    }

    private void OnArea3DBodyExited(Node3D body)
    {
        target = null;
    }

    private void LookAtTarget(Node3D target)
    {
        Vector3 dir = -(target.GlobalPosition - GlobalPosition).Normalized();
        float targetRotation = Mathf.Atan2(dir.X, dir.Z);
        Vector3 rot = _neck.Rotation;
        rot.Y = targetRotation;
        _neck.Rotation = rot;
    }
}