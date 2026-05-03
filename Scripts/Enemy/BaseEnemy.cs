using Godot;

public partial class BaseEnemy : CharacterBody3D
{

    [Export] private string[] _enemyGroups;


    public override void _Ready()
    {
        foreach (string group in _enemyGroups)
        {
            AddToGroup(group);
        }
    }

    public virtual void OnTriggered()
    {

    }
}