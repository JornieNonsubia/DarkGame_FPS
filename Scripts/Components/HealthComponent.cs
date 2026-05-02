using Godot;

public partial class HealthComponent : Node
{
    [Signal]
    public delegate void HealthChangedEventHandler(float newHealth, float maxHealth);

    [Signal]
    public delegate void DamageTakenEventHandler(float amount, Node3D source);

    [Signal]
    public delegate void DiedEventHandler();

    [Export] private float _maxHealth = 100f;
    [Export] private bool _startAtMax = true;

    private float _currentHealth;
    private bool _isAlive = true;

    public override void _Ready()
    {
        if (_startAtMax)
            _currentHealth = _maxHealth;
    }

    public void TakeDamage(float amount, Node3D source = null)
    {
        if (!_isAlive)
            return;

        float damage = Mathf.Max(0f, amount);
        _currentHealth = Mathf.Max(0f, _currentHealth - damage);
        EmitSignalDamageTaken(damage, source);
        EmitSignalHealthChanged(_currentHealth, _maxHealth);

        GD.Print(GetParent().Name + " took " + damage + " dmg. Health: " + _currentHealth + "/" + _maxHealth);

        if (_currentHealth <= 0)
            HandleDeath();
    }

    private void HandleDeath()
    {
        if (!_isAlive)
            return;

        _isAlive = false;
        _currentHealth = 0f;
        EmitSignalDied();
    }

    public void Heal(float amount)
    {
        if (!_isAlive)
            return;

        float heal = Mathf.Max(0f, amount);
        _currentHealth = Mathf.Min(_maxHealth, _currentHealth + heal);

        EmitSignalHealthChanged(_currentHealth, _maxHealth);
    }
}