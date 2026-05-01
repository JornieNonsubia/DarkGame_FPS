using Godot;
using Godot.Collections;

public partial class Projectile : Area3D
{
    private Vector3 _velocity;
    private float _damage;


    public override void _Ready()
    {
        BodyEntered += OnBodyEnteredEventHandler;

        GetTree().CreateTimer(3f).Timeout += () => QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        float fDelta = (float)delta;
        PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
        Vector3 from = GlobalPosition;
        Vector3 to = from + _velocity * fDelta;

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to);
        query.CollisionMask = 1;
        Dictionary result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            GlobalPosition = (Vector3)result["position"];
            OnBodyEnteredEventHandler((Node3D)result["collider"]);
            return;
        }

        GlobalPosition = to;
    }

    public void Setup(Vector3 velocity, float damage)
    {
        _velocity = velocity;
        _damage = damage;
    }

    public void OnBodyEnteredEventHandler(Node3D body)
    {
        GD.Print("Projectile hit: " + body.Name + " at " + GlobalPosition);
        SpawnImpactMarker(GlobalPosition);
        QueueFree();
    }
    
    private void SpawnImpactMarker(Vector3 position)
    {
        MeshInstance3D marker = new MeshInstance3D();
        BoxMesh box = new BoxMesh();
        box.Size = new Vector3(0.1f, 0.1f, 0.1f);
        marker.Mesh = box;

        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = Colors.Red;
        marker.SetSurfaceOverrideMaterial(0, material);

        GetTree().CurrentScene.AddChild(marker);
        marker.GlobalPosition = position;

        GetTree().CreateTimer(2f).Timeout += () => { marker.QueueFree(); };
    }

}