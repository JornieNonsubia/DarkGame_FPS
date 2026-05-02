using Godot;

[GlobalClass]
public partial class WeaponData : Resource
{
    [Export] private Weapon _weapon;
    [Export] private bool _unlocked = false;
    [Export] private int _ammo = 0;
    public Weapon Weapon { get => _weapon; set => _weapon = value; }
    public bool Unlocked { get => _unlocked; }
    public int Ammo { get => _ammo; set => _ammo = value; }

    public void Unlock()
    {
        _unlocked = true;
    }

    public void RefillAmmo(int amount)
    {
        if (amount > _weapon.MaxAmmo - _ammo)
            _ammo = _weapon.MaxAmmo;
        else
            _ammo += amount;
    }
}