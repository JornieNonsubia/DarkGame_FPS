using Godot;
using Godot.Collections;

// TODO Triggerable System
[Tool]
public partial class TriggerableComponent : Node
{
    [Export] private string _targetName = "";

    private void ApplyProperties(Dictionary entityProperties)
    {
        // _targetName = entityProperties[_targetName,""]
    }

    public override void _Ready()
    {
        CallDeferred("AddToGroup");
    }

    private void AddToGroup()
    {
        if(GetParent()!=null)
            GetParent().AddToGroup("MapEntities");
    }
}