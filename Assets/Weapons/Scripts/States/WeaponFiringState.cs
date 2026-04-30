using Godot;

public partial class WeaponFiringState : WeaponState
{
    public void OnFiringStateEntered()
    {
        WeaponController.FireWeapon();
    }

    public void OnFiringStateProcessing(double delta)
    {
        if (WeaponController == null)
            return;

        if (WeaponController.CurrentAmmo <= 0)
        {
            WeaponController.WeaponStateChart.Call("send_event", "onEmpty");
            return;
        }

        WeaponController.WeaponStateChart.Call("send_event", "onIdle");
    }
}