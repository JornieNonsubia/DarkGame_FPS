using Godot;

public partial class BuildItem : Resource
{
    [Export] public string Name;
    [Export] public PackedScene SceneToPlace; // Сцена самого об'єкта
    [Export] public PackedScene PreviewScene; // Прозора "фантомна" версія
}