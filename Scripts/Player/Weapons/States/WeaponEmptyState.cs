using Godot;

public partial class WeaponEmptyState : WeaponState
{
    public void OnEmptyStateEntered()
    {
        GD.Print("Weapon Empty!");

    }

    public void OnEmptyStateProcessing(double delta)
    {
        if (Input.IsActionJustPressed("WeaponReload"))
        {
            WeaponController.WeaponStateChart.Call("send_event", "onReloading");
        }
    }
}