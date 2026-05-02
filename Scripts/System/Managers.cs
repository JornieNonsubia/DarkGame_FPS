using Godot;

public partial class Managers : Node
{
    public static Managers Instance { get; private set; }
    
    public WeaponManager WeaponManager;

    public override void _Ready()
    {
        Instance = this;
        CallDeferred("FindManagers");
    }

    void FindManagers()
    {
        WeaponManager = (WeaponManager)GetTree().GetFirstNodeInGroup("WeaponManager");
    }

}