using Godot;

public partial class AmmoPickup : BasePickup
{
    [Export] private int _weaponSlot = 1;
    [Export] private int _ammoAmount = 10;


    public override bool CanPickup(PlayerController player)
    {
        if (!Managers.Instance.WeaponManager.Weapons.ContainsKey(_weaponSlot))
            return false;

        WeaponData weaponData = Managers.Instance.WeaponManager.Weapons[_weaponSlot];

        return weaponData.Unlocked && weaponData.Ammo < weaponData.Weapon.MaxAmmo;
    }

    public override void ApplyPickup(PlayerController body)
    {
        WeaponData weaponData = Managers.Instance.WeaponManager.Weapons[_weaponSlot];
        
        weaponData.RefillAmmo(_ammoAmount);
    }
}