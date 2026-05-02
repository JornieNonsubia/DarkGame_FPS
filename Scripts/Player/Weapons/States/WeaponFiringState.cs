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

        if (!WeaponController.HasAmmo())
        {
            WeaponController.WeaponStateChart.Call("send_event", "onEmpty");
            return;
        }


        if (WeaponController.CurrentWeapon.IsAutomatic)
        {
            if (Input.IsActionPressed("LMB"))
            {
                if (WeaponController.CanFire())
                    WeaponController.FireWeapon();
            }
            else
            {
                WeaponController.WeaponStateChart.Call("send_event", "onIdle");
            }
        }
        else
        {
            WeaponController.WeaponStateChart.Call("send_event", "onIdle");
        }

    }
}