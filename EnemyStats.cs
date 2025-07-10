using UnityEngine;

// Manages enemy stats and health bar for a 3D action RPG
public class EnemyStats : MonoBehaviour
{
    // Base stats for health, mana, damage, and cooldown
    public float baseHealth = 50f; // Base HP without Strength
    public float healthPerStrength = 5f; // HP gained per Strength point
    public float baseMana = 30f; // Base MP without Intelligence
    public float manaPerIntelligence = 3f; // MP gained per Intelligence point
    public float baseDamage = 5f; // Base counter-attack damage
    public float damagePerStrength = 1f; // Damage gained per Strength point
    public float baseSpellDamage = 10f; // Base spell damage
    public float spellDamagePerIntelligence = 2f; // Spell damage per Intelligence
    public float baseAttackCooldown = 1f; // Base counter-attack cooldown
    public float cooldownReductionPerAgility = 0.05f; // Cooldown reduction per Agility

    // Core attributes
    public float strength = 5f; // Strength attribute
    public float agility = 5f; // Agility attribute
    public float intelligence = 5f; // Intelligence attribute

    // Health bar reference
    public GameObject healthBarPrefab; // Health bar prefab
    private float currentHealth;
    private float currentMana;
    private UIEnemyHealthBar healthBarInstance;

    // Calculated properties for max health and mana
    public float MaxHealth { get { return baseHealth + strength * healthPerStrength; } }
    public float MaxMana { get { return baseMana + intelligence * manaPerIntelligence; } }
    public float CurrentHealth { get { return currentHealth; } set { currentHealth = value; } } // Allow setting for combat
    public float CurrentMana { get { return currentMana; } }
    public float SpellDamage { get { return baseSpellDamage + intelligence * spellDamagePerIntelligence; } }

    // Initialize stats and health bar
    void Start()
    {
        currentHealth = MaxHealth;
        currentMana = MaxMana;
        if (healthBarPrefab != null)
        {
            GameObject healthBarObj = Instantiate(healthBarPrefab, transform.position, Quaternion.identity);
            healthBarInstance = healthBarObj.GetComponentInChildren<UIEnemyHealthBar>();
            if (healthBarInstance != null)
            {
                healthBarInstance.enemyStats = this;
                healthBarInstance.target = transform;
            }
        }
        Debug.Log($"{gameObject.name} Stats - HP: {currentHealth}/{MaxHealth}, MP: {currentMana}/{MaxMana}");
    }

    // Reduce mana by a specified amount
    public void UseMana(float amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0f, MaxMana);
        Debug.Log($"{gameObject.name} mana: {currentMana}/{MaxMana}");
    }

    // Get health as a percentage for UI
    public float GetHealthPercentage()
    {
        return currentHealth / MaxHealth;
    }

    // Get mana as a percentage for UI
    public float GetManaPercentage()
    {
        return currentMana / MaxMana;
    }

    // Clean up health bar on enemy destruction
    void OnDestroy()
    {
        if (healthBarInstance != null)
        {
            Destroy(healthBarInstance.gameObject);
        }
    }
}