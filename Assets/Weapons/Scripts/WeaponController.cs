using Godot;
using Godot.Collections;
using GodotStateCharts;

public partial class WeaponController : Node
{
    [Export] private Camera3D _camera;
    [Export] private Weapon _currentWeapon;
    [Export] private Node3D _weaponModelParent;
    [Export] private Node _weaponStateChart;

    public Node WeaponStateChart { get => _weaponStateChart; }

    private Node3D _currentWeaponModel;
    private int _currentAmmo;
    public int CurrentAmmo { get => _currentAmmo; }

    public override void _Ready()
    {
        SpawnWeaponModel();
        _currentAmmo = _currentWeapon.MaxAmmo;
    }

    private void SpawnWeaponModel()
    {
        if (_currentWeaponModel != null)
        {
            _currentWeaponModel.QueueFree(); // Removing Weapon Model
        }

        if (_currentWeapon.WeaponModel != null)
        {
            _currentWeaponModel = _currentWeapon.WeaponModel.Instantiate() as Node3D;
            _weaponModelParent.AddChild(_currentWeaponModel);
            _currentWeaponModel!.Position = _currentWeapon.WeaponPosition;
        }
    }

    public bool CanFire()
    {
        return _currentAmmo > 0;
    }

    public void FireWeapon()
    {
        if (CanFire())
        {
            _currentAmmo--;
            
            GD.Print("Fired! Ammo: ", _currentAmmo);

            if (_currentWeapon.IsHitScan)
            {
                PerformHitscan();
            }
            else
            {
                SpawnProjectile();
            }
            
        }
    }

    private void SpawnProjectile()
    {
        if(_currentWeapon.ProjectileModel==null || _camera == null)
            return;
        Projectile projectile = _currentWeapon.ProjectileModel.Instantiate() as Projectile;
        GetTree().CurrentScene.AddChild(projectile);

        projectile!.GlobalPosition = _camera.GlobalPosition;

        Vector3 forward = -_camera.GlobalTransform.Basis.Z;
        Vector3 vel = forward * _currentWeapon.ProjectileSpeed;
        projectile.LookAt(projectile.GlobalPosition+forward,Vector3.Up);
        
        projectile.Setup(vel,_currentWeapon.Damage);
    }

    private void PerformHitscan()
    {
        if (_camera == null)
            return;

        PhysicsDirectSpaceState3D spaceState = _camera.GetWorld3D().DirectSpaceState;
        Vector3 from = _camera.GlobalPosition;
        Vector3 forward = -_camera.GlobalTransform.Basis.Z;
        Vector3 to = from + forward * _currentWeapon.Range;

        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to);
        Dictionary result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            //CollisionShape3D resCol = (CollisionShape3D)result["collider"];
            Vector3 resPos = (Vector3)result["position"];
            GD.Print("Hit: &&&"  + " at " + resPos);
            SpawnImpactMarker(resPos);
        }
    }

    private void SpawnImpactMarker(Vector3 position)
    {
        MeshInstance3D marker = new MeshInstance3D();
        BoxMesh box = new BoxMesh();
        box.Size = new Vector3(0.1f, 0.1f, 0.1f);
        marker.Mesh = box;

        StandardMaterial3D material = new StandardMaterial3D();
        material.AlbedoColor = Colors.Red;
        marker.SetSurfaceOverrideMaterial(0, material);

        GetTree().CurrentScene.AddChild(marker);
        marker.GlobalPosition = position;

        GetTree().CreateTimer(2f).Timeout += () => { marker.QueueFree(); };
    }
}