using Godot;
using Godot.Collections;

public partial class StepHandlerComponent : Node
{
    [ExportCategory("References")]
    [Export] private PlayerController _player;

    [ExportCategory("Step Settings")]
    [Export] private float _surfaceThreshold = 0.3f;
    [Export] public float StepHeight = 0.5f;

    private string _stepStatus;

    private const float FeetAdjustedHeight = 0.05f;
    private const float MinStepHeight = 0.1f;
    private const float MinMovementLength = 0.1f;
    private const float MinDotValue = 0.5f;

    public string StepStatus { get => _stepStatus; }

    public void HandleStepClimbing()
    {
        _stepStatus = "No Vertical Collisions Detected";

        for (int i = 0; i < _player.GetSlideCollisionCount(); i++)
        {
            KinematicCollision3D collision = _player.GetSlideCollision(i);
            if (IsVerticalSurface(collision))
            {
                float measuredHeight = MeasureStepHeight(collision);
                if (measuredHeight > MinStepHeight && measuredHeight <= StepHeight && IsValidStepDirection(collision))
                {
                    Vector3 playerGlobalPosition = _player.GlobalPosition;
                    playerGlobalPosition.Y += measuredHeight;
                    _player.GlobalPosition = playerGlobalPosition;
                    _player.Velocity = _player.PreviousVelocity;
                    _player.Head.SmoothStep(measuredHeight);
                    _stepStatus = "Step Found! Height: " + measuredHeight.ToString();
                }
                else
                {
                    _stepStatus = "Step Too High: " + measuredHeight.ToString();
                }
                break;
            }
        }
    }



    private bool IsVerticalSurface(KinematicCollision3D collision)
    {
        Vector3 normal = collision.GetNormal();
        if (Mathf.Abs(normal.Y) <= _surfaceThreshold)
        {
            _stepStatus = "CollisionShape: Vertical Collision Found! " + normal.ToString();
            return true;
        }
        return CheckCollisionSurface(collision);
    }

    private bool CheckCollisionSurface(KinematicCollision3D collision)
    {
        PhysicsDirectSpaceState3D spaceState = _player.GetWorld3D().DirectSpaceState;
        Vector3 collisionPoint = collision.GetPosition();

        var playerFeet = GetPlayerFeetPosition();
        collisionPoint.Y = playerFeet.Y;

        var query = PhysicsRayQueryParameters3D.Create(playerFeet, collisionPoint);
        query.SetCollisionMask(_player.CollisionMask);
        query.Exclude = new Array<Rid> { _player.GetRid() };

        Dictionary result = spaceState.IntersectRay(query);
        if (result.Count > 0)
        {
            Vector3 resultNormal = (Vector3)result["normal"];
            if (Mathf.Abs(resultNormal.Y) <= _surfaceThreshold)
            {
                _stepStatus = "Raycast: Vertical Collision Found! " + resultNormal.ToString();
                return true;
            }
        }
        _stepStatus = "No Vertical Collision Detected.";
        return false;
    }

    private Vector3 GetPlayerFeetPosition()
    {
        Vector3 feetPos = _player.GlobalPosition;

        float shapeHeight = ((CapsuleShape3D)_player.StandingCollision.Shape).Height;
        feetPos.Y -= shapeHeight / 2;
        feetPos.Y += FeetAdjustedHeight;
        return feetPos;
    }

    private float MeasureStepHeight(KinematicCollision3D collision)
    {
        PhysicsDirectSpaceState3D spaceState = _player.GetWorld3D().DirectSpaceState;
        Vector3 collisionPoint = collision.GetPosition();

        Vector3 playerFeet = GetPlayerFeetPosition();
        float shapeHeight = ((CapsuleShape3D)_player.StandingCollision.Shape).Height;

        float playerHeadY = _player.GlobalPosition.Y + (shapeHeight / 2);

        Vector3 rayStart = new Vector3(collisionPoint.X, playerHeadY, collisionPoint.Z);
        Vector3 rayEnd = new Vector3(collisionPoint.X, playerFeet.Y, collisionPoint.Z);

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(rayStart, rayEnd);
        query.SetCollisionMask(_player.CollisionMask);
        query.Exclude = new Array<Rid> { _player.GetRid() };

        Dictionary result = spaceState.IntersectRay(query);
        if (result.Count > 0)
        {
            Vector3 intersectionPoint = (Vector3)result["position"];
            return intersectionPoint.Y - playerFeet.Y;
        }
        return 0f;
    }

    private bool IsValidStepDirection(KinematicCollision3D collision)
    {
        Vector3 collisionNormal = collision.GetNormal();
        Vector2 inputDir = _player.InputDir;
        Vector3 movementDir = _player.Transform.Basis * new Vector3(inputDir.X, 0f, inputDir.Y);

        if (movementDir.Length() > MinMovementLength)
        {
            movementDir = movementDir.Normalized();
            var dotProduct = movementDir.Dot(-collisionNormal);
            return dotProduct > MinDotValue;
        }
        return false;
    }
}