using Godot;
using GDC = Godot.Collections;

public partial class WeaponManager : Node
{
    [Export] private GDC.Dictionary<int, WeaponData> _weapons;
    [Export] private PlayerController _player;
    [Export] private CanvasLayer UI;

    public GDC.Dictionary<int, WeaponData> Weapons { get => _weapons; }

    public int CurrentSlot = 1;

    public override void _Ready()
    {
        AddToGroup("WeaponManager");

        CallDeferred("InitializeStartingWeapon");
    }

    public override void _Process(double delta)
    {
        Label nameLabel = (Label)UI.GetNode("WeaponName");

        nameLabel.Text = _weapons[CurrentSlot].Weapon.WeaponName + "\n" + _weapons[CurrentSlot].CurrentAmmo + "/"
                         + _weapons[CurrentSlot].ReserveAmmo;
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
        {
            _weapons[slot].CurrentAmmo = Mathf.Max(0, _weapons[slot].CurrentAmmo - amount);

        }
    }

    public void Reload(int slot)
    {
        if (!_weapons.ContainsKey(slot))
            return;

        int ammoNeeded = _weapons[slot].Weapon.MagazineSize - _weapons[slot].CurrentAmmo;
        int ammoToTransfer = Mathf.Min(ammoNeeded, _weapons[slot].ReserveAmmo);

        _weapons[slot].CurrentAmmo += ammoToTransfer;
        _weapons[slot].ReserveAmmo -= ammoToTransfer;
    }

    public int GetCurrentAmmo()
    {
        return _weapons[CurrentSlot].CurrentAmmo;
    }

    void InitializeStartingWeapon()
    {
        SwitchToSlot(1); // Switching to a weapon in a first slot
    }

    public void UnlockWeapon(int slot, Weapon weaponResource)
    {
        _weapons[slot].Weapon = weaponResource;
        _weapons[slot].Unlock();
        _weapons[slot].ReserveAmmo = weaponResource.MaxAmmo - weaponResource.MagazineSize;
        _weapons[slot].CurrentAmmo = weaponResource.MagazineSize;
    }
}