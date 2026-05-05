using Godot;
using System;

public partial class WeaponReloadingState : WeaponState
{
    public void OnReloadingStateEntered()
    {
        GD.Print("On Reloading Entered");

    }

    public void OnReloadingStateProcessing(float delta)
    {
        GD.Print("Reloading");
        Managers.Instance.WeaponManager.Reload(Managers.Instance.WeaponManager.CurrentSlot);
        WeaponController.WeaponStateChart.Call("send_event", "onIdle");

    }

    public void OnReloadingStateExited()
    {
        GD.Print("On Reloading Exited");

    }

}