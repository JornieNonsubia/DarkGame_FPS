using Godot;

public partial class InteractionRaycast : RayCast3D
{
    private GodotObject currentObject;

    public override void _Process(double delta)
    {
        if (IsColliding())
        {
            GodotObject targetObject = GetCollider();
            if (targetObject == currentObject)
                return;
            else
                currentObject = targetObject;
        }
        else
            currentObject = null;
    }
}