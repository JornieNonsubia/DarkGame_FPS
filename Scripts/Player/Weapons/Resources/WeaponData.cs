using Godot;

[GlobalClass]
public partial class WeaponData : Resource
{
    [Export] private Weapon _weapon;
    [Export] private bool _unlocked = false;
    [Export] private int _currentAmmo = 0;
    [Export] private int _reserveAmmo = 0;
    public Weapon Weapon { get => _weapon; set => _weapon = value; }
    public bool Unlocked { get => _unlocked; }
    public int CurrentAmmo { get => _currentAmmo; set => _currentAmmo = value; }
    public int ReserveAmmo { get => _reserveAmmo; set => _reserveAmmo = value; }

    public void Unlock()
    {
        _unlocked = true;
    }

    public void RefillAmmo(int amount)
    {
        if (amount > _weapon.MaxAmmo - _currentAmmo + _reserveAmmo)
            _reserveAmmo = _weapon.MaxAmmo - _currentAmmo;
        else
            _reserveAmmo += amount;
    }
}