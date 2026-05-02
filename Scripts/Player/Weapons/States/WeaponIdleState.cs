using Godot;

public partial class WeaponIdleState : WeaponState
{
    public void OnIdleStateProcessing(float delta)
    {
        if (WeaponController == null)
            return;

        if (Input.IsActionJustPressed("LMB") && WeaponController.CanFire())
        {
            WeaponController.WeaponStateChart.Call("send_event", "onFiring");
        }

        if (!WeaponController.HasAmmo())
        {
            WeaponController.WeaponStateChart.Call("send_event", "onEmpty");
        }
    }
}