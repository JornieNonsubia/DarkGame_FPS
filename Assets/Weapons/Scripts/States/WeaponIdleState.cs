using Godot;

public partial class WeaponIdleState : WeaponState
{
    public void OnIdleStateProcessing(float delta)
    {
        if (WeaponController == null)
            return;

        if (Input.IsActionJustPressed("LMB") && WeaponController.CanFire())
        {
            GD.Print("Idle LMB Pressed IF");
            WeaponController.WeaponStateChart.Call("send_event", "onFiring");
        }

        if (!WeaponController.CanFire())
        {
            GD.Print("Idle CanFire IF");
            
            WeaponController.WeaponStateChart.Call("send_event", "onEmpty");
        }
    }
}