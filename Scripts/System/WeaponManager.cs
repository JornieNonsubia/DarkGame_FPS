using Godot;
using GDC = Godot.Collections;

public partial class WeaponManager : Node
{
    [Export] private GDC.Dictionary<int, WeaponData> _weapons;
    [Export] private PlayerController _player;

    public GDC.Dictionary<int, WeaponData> Weapons { get => _weapons; }

    public int CurrentSlot = 1;

    public override void _Ready()
    {
        AddToGroup("WeaponManager");

        CallDeferred("InitializeStartingWeapon");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventKey eventKey)
        {
            if (eventKey.IsActionPressed("WeaponSlot_1"))
                SwitchToSlot(1);

            if (eventKey.IsActionPressed("WeaponSlot_2"))
                SwitchToSlot(2);
        }
    }

    public void SwitchToSlot(int slot)
    {
        WeaponData weaponData = _weapons[slot];

        if (weaponData != null && weaponData.Unlocked)
        {
            CurrentSlot = slot;
            _player.WeaponController.SwitchWeapon(weaponData);
        }
    }

    public void UseAmmo(int slot, int amount = 1)
    {
        if (_weapons.ContainsKey(slot))
            _weapons[slot].Ammo = Mathf.Max(0, _weapons[slot].Ammo - amount);
    }

    public int GetCurrentAmmo()
    {
        return _weapons[CurrentSlot].Ammo;
    }

    void InitializeStartingWeapon()
    {
        SwitchToSlot(1); // Switching to a weapon in a first slot
    }

    public void UnlockWeapon(int weaponSlot, Weapon weaponResource)
    {
        _weapons[weaponSlot].Weapon = weaponResource;
        _weapons[weaponSlot].Unlock();
        _weapons[weaponSlot].Ammo = weaponResource.MaxAmmo;
    }
}