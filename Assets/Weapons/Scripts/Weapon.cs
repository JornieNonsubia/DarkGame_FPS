using Godot;

public partial class Weapon : Resource
{
    [Export] private string _weaponName = "Pistol";
    [Export] private float _damage = 25f;
    [Export] private int _maxAmmo = 12;
    [Export] private float _range = 25;
    [Export] private bool _isHitscan = true;
    [Export] private PackedScene _weaponModel;
    [Export] private PackedScene _projectileModel;
    [Export] private Vector3 _weaponPosition = new Vector3(0.2f, -0.2f, -0.3f);

    public string WeaponName { get => _weaponName; }
    public float Damage { get => _damage; }
    public int MaxAmmo { get => _maxAmmo; }
    public float Range { get => _range; }
    public bool IsHitScan { get => _isHitscan; }
    public PackedScene WeaponModel { get => _weaponModel; }
    public PackedScene ProjectileModel { get => _projectileModel; }
    public Vector3 WeaponPosition { get => _weaponPosition; }

}