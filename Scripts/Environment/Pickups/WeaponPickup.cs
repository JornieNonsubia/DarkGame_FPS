using Godot;

public partial class WeaponPickup : BasePickup
{
    [Export] private int _weaponSlot;
    [Export] private Weapon _weaponResource;

    public override void ApplyPickup(PlayerController body)
    {
        WeaponData weaponData = Managers.Instance.WeaponManager.Weapons[_weaponSlot];

        if (weaponData.Unlocked)
        {
            weaponData.ReserveAmmo = _weaponResource.MaxAmmo - _weaponResource.MagazineSize;
            weaponData.CurrentAmmo = _weaponResource.MagazineSize;
            GD.Print("Ammo refilled: " + _weaponResource.WeaponName);
        }
        else
        {
            Managers.Instance.WeaponManager.UnlockWeapon(_weaponSlot, _weaponResource);
            Managers.Instance.WeaponManager.SwitchToSlot(_weaponSlot);
            GD.Print("Unlocked: " + _weaponResource.WeaponName);
        }
    }

    public override bool CanPickup(PlayerController player)
    {
        WeaponData weaponData = Managers.Instance.WeaponManager.Weapons[_weaponSlot];

        return !weaponData.Unlocked || weaponData.ReserveAmmo + weaponData.CurrentAmmo < _weaponResource.MaxAmmo;
    }
}