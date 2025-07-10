using UnityEngine;

// Manages combat-related functionality for an enemy, including taking damage and optional counter-attacking
public class EnemyCombat : MonoBehaviour
{
    private EnemyStats enemyStats; // Reference to the EnemyStats component for stat access
    private float lastAttackTime; // Tracks the time of the last counter-attack
    public bool canCounterAttack = true; // Toggles whether the enemy can counter-attack

    // Properties derived from EnemyStats for combat calculations
    public float CounterAttackDamage { get { return enemyStats.baseDamage + enemyStats.strength * enemyStats.damagePerStrength; } }
    public float AttackCooldown { get { return Mathf.Max(0.2f, enemyStats.baseAttackCooldown - enemyStats.agility * enemyStats.cooldownReductionPerAgility); } }

    // Initialize by getting the EnemyStats component
    void Start()
    {
        enemyStats = GetComponent<EnemyStats>();
        if (enemyStats == null)
        {
            Debug.LogError($"{gameObject.name} requires an EnemyStats component for combat functionality.");
        }
    }

    // Handles damage taken by the enemy and triggers counter-attack if enabled and cooldown allows
    public void TakeDamage(float damage)
    {
        if (enemyStats == null) return; // Safety check for EnemyStats reference

        // Apply damage to the enemy's health
        enemyStats.CurrentHealth = Mathf.Clamp(enemyStats.CurrentHealth - damage, 0f, enemyStats.MaxHealth);
        Debug.Log($"{gameObject.name} health: {enemyStats.CurrentHealth}/{enemyStats.MaxHealth}");

        // Perform counter-attack if enabled and cooldown has elapsed
        if (canCounterAttack && Time.time >= lastAttackTime + AttackCooldown)
        {
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(CounterAttackDamage);
                lastAttackTime = Time.time;
                Debug.Log($"{gameObject.name} counter-attacked for {CounterAttackDamage} damage.");
            }
        }

        // Destroy the enemy if health reaches zero
        if (enemyStats.CurrentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}