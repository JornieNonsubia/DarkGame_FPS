using Godot;
using System;

public partial class TurretFireComponent : Node
{
    [Export] private float _damage = 15;
    [Export] private float _fireRate = 2;
    [Export] private int _ammo = 100;

    private double _fireTimer = 0;

    public override void _Process(double delta)
    {
        if (!CanFire())
            _fireTimer -= delta;
    }

    public void ApplyDamageToTarget(Node3D target)
    {
        HealthComponent healthComponent = (HealthComponent)target.GetNodeOrNull("HealthComponent");

        if (healthComponent != null && healthComponent.HasMethod("TakeDamage")&& CanFire())
        {
            _fireTimer = 1 / _fireRate;
            healthComponent.TakeDamage(_damage, GetOwner() as Node3D);
        }
    }

    private bool CanFire()
    {
        return _fireTimer <= 0;
    }
}