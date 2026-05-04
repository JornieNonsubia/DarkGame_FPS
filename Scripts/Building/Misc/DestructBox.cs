using Godot;
using System;

public partial class DestructBox : StaticBody3D
{
    public void OnHealthComponentDied()
    {
        GD.Print(Name,"Destroyed");
        QueueFree();
    }
}
