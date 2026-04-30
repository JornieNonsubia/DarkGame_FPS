using Godot;

public partial class WeaponEmptyState : WeaponState
{
    public void OnEmptyStateEntered()
    {
        GD.Print("Weapon Empty!");

    }

    public void OnEmptyStateProcessing(double delta)
    {
        //TODO: Reload
    }
}