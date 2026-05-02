using Godot;

public partial class Weapon : Resource
{
    [Export] private string _weaponName = "Pistol";
    [Export] private PackedScene _weaponModel;
    [Export] private Vector3 _weaponPosition = new Vector3(0.2f, -0.2f, -0.3f);
    [Export] private float _damage = 25f;
    [Export] private float _fireRate = 2; //shot per sec
    [Export] private bool _isAutomatic = false;
    [Export] private int _maxAmmo = 12;
    [Export] private float _range = 25;
    [Export(PropertyHint.Range, "0,100")] private int _accuracy = 100;
    [Export] private int _pelletCount = 1;
    [Export] private float _pelletSpread = 0f;
    [Export] private bool _isHitscan = true;
    [ExportCategory("Projectile Settings")]
    [Export] private float _projectileSpeed = 50;
    [Export] private PackedScene _projectileModel;


    public string WeaponName { get => _weaponName; }
    public PackedScene WeaponModel { get => _weaponModel; }
    public Vector3 WeaponPosition { get => _weaponPosition; }
    public float Damage { get => _damage; }
    public float FireRate { get => _fireRate; }
    public bool IsAutomatic { get => _isAutomatic; }
    public int MaxAmmo { get => _maxAmmo; }
    public float Range { get => _range; }
    public int Accuracy { get => _accuracy; }
    public int PelletCount { get => _pelletCount; }
    public float PelletSpread { get => _pelletSpread; }
    public bool IsHitScan { get => _isHitscan; }
    public float ProjectileSpeed { get => _projectileSpeed; }
    public PackedScene ProjectileModel { get => _projectileModel; }
}