using Godot;

public partial class BuildItem : Resource
{
    [Export] public string Name;
    [Export] public PackedScene SceneToPlace; // Object
    [Export] public PackedScene PreviewScene; // Fantom preview version 
}