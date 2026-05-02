using Godot;
using Godot.Collections;

public partial class WeaponController : Node
{
    [Export] private Camera3D _camera;
    [Export] private Node3D _weaponModelParent;
    [Export] private Node _weaponStateChart;
    
    private Weapon _currentWeapon;
    private Node3D _currentWeaponModel;
    private bool _canFire = true;
    private float _fireRateTimer = 0f;
    public Node WeaponStateChart { get => _weaponStateChart; }
    public Weapon CurrentWeapon { get => _currentWeapon; }


    public override void _Ready()
    {
        //SpawnWeaponModel();
    }

    public override void _Process(double delta)
    {
        float fDelta = (float)delta;

        if (_fireRateTimer > 0)
        {
            _fireRateTimer -= fDelta;

            if (_fireRateTimer <= 0)
            {
                _canFire = true;
            }
        }
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
        WeaponData wpnData = Managers.Instance.WeaponManager.Weapons[Managers.Instance.WeaponManager.CurrentSlot];
        return wpnData.Ammo > 0 && _canFire;
    }

    public void FireWeapon()
    {
        if (CanFire())
        {
           Managers.Instance.WeaponManager.UseAmmo(Managers.Instance.WeaponManager.CurrentSlot);


            _canFire = false;
            _fireRateTimer = 1f / _currentWeapon.FireRate;

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
        if (_currentWeapon.ProjectileModel == null || _camera == null)
            return;

        Projectile projectile = _currentWeapon.ProjectileModel.Instantiate() as Projectile;
        GetTree().CurrentScene.AddChild(projectile);

        projectile!.GlobalPosition = _camera.GlobalPosition;


        Vector3 forward = -_camera.GlobalTransform.Basis.Z;

        float accuracySpread = (100 - _currentWeapon.Accuracy) / 1000f;
        float accuracyX = (float)GD.RandRange(-accuracySpread, accuracySpread);
        float accuracyY = (float)GD.RandRange(-accuracySpread, accuracySpread);
        Vector3 direction = forward + new Vector3(accuracyX, accuracyY, 0) * _camera.GlobalTransform.Basis;

        Vector3 vel = direction * _currentWeapon.ProjectileSpeed;
        projectile.LookAt(projectile.GlobalPosition + direction, Vector3.Up);

        projectile.Setup(vel, _currentWeapon.Damage);
    }

    private void PerformHitscan()
    {
        if (_camera == null)
            return;

        PhysicsDirectSpaceState3D spaceState = _camera.GetWorld3D().DirectSpaceState;
        Vector3 from = _camera.GlobalPosition;
        float accuracySpread = (100 - _currentWeapon.Accuracy) / 1000f;
        // Accuracy random
        float accuracyX = (float)GD.RandRange(-accuracySpread, accuracySpread);
        float accuracyY = (float)GD.RandRange(-accuracySpread, accuracySpread);

        for (int i = 0; i < _currentWeapon.PelletCount; i++)
        {
            Vector3 forward = -_camera.GlobalTransform.Basis.Z;

            Vector3 direction = forward + new Vector3(accuracyX, accuracyY, 0) * _camera.GlobalTransform.Basis;

            if (CurrentWeapon.PelletCount > 1)
            {
                float spreadX = (float)GD.RandRange(
                    -(100 - _currentWeapon.PelletSpread) / 1000f,
                    (100 - _currentWeapon.PelletSpread) / 1000f
                );

                float spreadY = (float)GD.RandRange(
                    -(100 - _currentWeapon.PelletSpread) / 1000f,
                    (100 - _currentWeapon.PelletSpread) / 1000f
                );

                direction += new Vector3(spreadX, spreadY, 0);
            }

            Vector3 to = from + direction * _currentWeapon.Range;

            PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(from, to);
            Dictionary result = spaceState.IntersectRay(query);

            if (result.Count > 0)
            {
                //CollisionShape3D resCol = (CollisionShape3D)result["collider"];
                Vector3 resPos = (Vector3)result["position"];
                GD.Print("Hit: &&&" + " at " + resPos);
                SpawnImpactMarker(resPos);
                ApplyDamageToTarget((Node3D)result["collider"]);
            }
        }
    }

    private void ApplyDamageToTarget(Node3D target)
    {
        HealthComponent healthComponent = (HealthComponent)target.GetNodeOrNull("HealthComponent");

        if (healthComponent != null && healthComponent.HasMethod("TakeDamage"))
            healthComponent.TakeDamage(_currentWeapon.Damage, GetOwner() as Node3D);
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

    public void SwitchWeapon(WeaponData weaponData)
    {
        _currentWeapon = weaponData.Weapon;

        if (_currentWeaponModel != null)
            _currentWeaponModel.QueueFree();

        SpawnWeaponModel();
        _weaponStateChart.Call("send_event", "onIdle");
    }

    public bool HasAmmo()
    {
        WeaponData wpnData = Managers.Instance.WeaponManager.Weapons[Managers.Instance.WeaponManager.CurrentSlot];
        return wpnData.Ammo > 0;
    }
    
    
}