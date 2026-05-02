using Godot;

public partial class BasePickup : Area3D
{
    [Export] public float RotationSpeed = 60f;
    [Export] public float FloatHeight = 0.1f;
    [Export] public float FloatSpeed = 2f;

    private float _startY;
    private float _time = 0;

    public override void _Ready()
    {
        BodyEntered += OnPickup;
        _startY = Position.Y;
    }

    public override void _Process(double delta)
    {
        float fDelta = (float)delta;
        _time += fDelta;

        Vector3 pos = Position;
        pos.Y = _startY + Mathf.Sin(_time * FloatSpeed) * FloatHeight;
        Position = pos;

        RotateY(Mathf.DegToRad(RotationSpeed) * fDelta);
    }

    private void OnPickup(Node3D body)
    {
        if (body.GetType() != typeof(PlayerController))
            return;

        if (CanPickup((PlayerController)body))
        {
            ApplyPickup((PlayerController)body);
            QueueFree();
        }
    }

    public virtual void ApplyPickup(PlayerController body)
    {
        throw new NotImplementedException();
    }

    public virtual bool CanPickup(PlayerController player)
    {
        return true;
    }
}