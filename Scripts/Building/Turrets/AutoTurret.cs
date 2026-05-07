using Godot;

public partial class AutoTurret : StaticBody3D
{
    [Export] private Node3D _head;
    [Export] private Node3D _neck;
    [Export] private RayCast3D _ray;
    [Export] private TurretFireComponent _turretFireComponent;
    [Export] private float _rotationSpeed = 5f;

    [ExportGroup("Detection area")]
    [Export] private float _detectionAreaAngleWidth = 120;
    [Export] private float _detectionAreaAngleHeight = 50;
    [Export] private float _detectionAreaRange = 100f;

    private ConvexPolygonShape3D _detectionArea;
    private List<Node3D> _targets = [];
    private Node3D _target;


    public override void _Ready()
    {
        InitializeDetectionArea();
        GetNode<CollisionShape3D>("Area3D/DetectionShape").Shape = _detectionArea;

        _ray.SetTargetPosition(new Vector3(0, 0, -_detectionAreaRange));
    }
    
    public override void _PhysicsProcess(double delta) 
    {
        // Delete dead targets
        _targets.RemoveAll(t => !IsInstanceValid(t));

        if (_targets.Count == 0)
        {
            _target = null;
            return;
        }

        //Searching for the closest one
        _target = _targets.OrderBy(t => GlobalPosition.DistanceSquaredTo(t.GlobalPosition)).First();
    
        if (_target != null)
        {
            RotateTowardsTarget(_target, (float)delta);
            _turretFireComponent.ApplyDamageToTarget(_target);
        }

    }

    private void OnArea3DBodyEntered(Node3D body)
    {
        if (body.IsInGroup("Enemies"))
            _targets.Add(body);

        _target = body;
    }

    private void OnArea3DBodyExited(Node3D body)
    {
        _targets.Remove(body);

        if (_target == body)
            _target = null;
    }

    private void RotateTowardsTarget(Node3D target, float delta)
    {
        Vector3 targetPos = target.GlobalPosition;
        Vector3 neckRot = _neck.Rotation;
        
        Vector3 localPosForNeck = _neck.GlobalTransform.AffineInverse() * targetPos;
        
        float angleY = Mathf.Atan2(localPosForNeck.X, localPosForNeck.Z);
        
        neckRot.Y = Mathf.LerpAngle(neckRot.Y, neckRot.Y + angleY + Mathf.Pi, _rotationSpeed * delta);
        _neck.Rotation = neckRot;
        
        
        Vector3 localPosForHead = _head.GlobalTransform.AffineInverse() * targetPos;
        Vector3 headRot = _head.Rotation;
        
        float angleX = Mathf.Atan2(localPosForHead.Y, -localPosForHead.Z);
        headRot.X = Mathf.LerpAngle(headRot.X, headRot.X + angleX, _rotationSpeed * delta);
    }

    private void InitializeDetectionArea()
    {
        // Піраміда: 1 вершина + 4 точки прямокутної основи
        Vector3[] points = new Vector3[5];

        float halfRadW = Mathf.DegToRad(_detectionAreaAngleWidth / 2f);
        float halfRadH = Mathf.DegToRad(_detectionAreaAngleHeight / 2f);

        float xOffset = _detectionAreaRange * Mathf.Tan(halfRadW);
        float yOffset = _detectionAreaRange * Mathf.Tan(halfRadH);
        float zOffset = -_detectionAreaRange;

        // Initial point
        points[0] = Vector3.Zero;

        points[1] = new Vector3(-xOffset, yOffset, zOffset);
        points[2] = new Vector3(xOffset, yOffset, zOffset);
        points[3] = new Vector3(-xOffset, -yOffset, zOffset);
        points[4] = new Vector3(xOffset, -yOffset, zOffset);

        _detectionArea = new ConvexPolygonShape3D();
        _detectionArea.Points = points;
    }
}