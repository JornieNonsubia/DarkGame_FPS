using Godot;

public partial class InteractionRaycast : RayCast3D
{
    private GodotObject _currentObject;

    public override void _Process(double delta)
    {
        if (IsColliding())
        {
            GodotObject targetObject = GetCollider();

            if (targetObject == _currentObject)
                return;
            else
                _currentObject = targetObject;
        }
        else
            _currentObject = null;
    }
}