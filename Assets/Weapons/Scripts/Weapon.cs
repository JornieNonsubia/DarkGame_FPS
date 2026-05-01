using Godot;

public partial class Weapon : Resource
{
    [Export] private string _weaponName = "Pistol";
    [Export] private PackedScene _weaponModel;
    [Export] private Vector3 _weaponPosition = new Vector3(0.2f, -0.2f, -0.3f);
    [Export] private float _damage = 25f;
    [Export] private int _maxAmmo = 12;
    [Export] private float _range = 25;
    [Export] private bool _isHitscan = true;
    [ExportCategory("Projectile Settings")]
    [Export] private float _projectileSpeed = 50;
    [Export] private PackedScene _projectileModel;


    public string WeaponName { get => _weaponName; }
    public PackedScene WeaponModel { get => _weaponModel; }
    public Vector3 WeaponPosition { get => _weaponPosition; }
    public float Damage { get => _damage; }
    public int MaxAmmo { get => _maxAmmo; }
    public float Range { get => _range; }
    public bool IsHitScan { get => _isHitscan; }
    public float ProjectileSpeed { get => _projectileSpeed; }
    public PackedScene ProjectileModel { get => _projectileModel; }
}