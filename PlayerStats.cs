using UnityEngine;

// Manages player stats for health, mana, damage, and movement
public class PlayerStats : MonoBehaviour
{
    // Base stats for health, mana, and damage
    public float baseHealth = 100f; // Base HP without Strength
    public float healthPerStrength = 10f; // HP gained per Strength point
    public float baseMana = 50f; // Base MP without Intelligence
    public float manaPerIntelligence = 5f; // MP gained per Intelligence point
    public float baseDamage = 10f; // Base damage without Strength
    public float damagePerStrength = 2f; // Damage gained per Strength point
    public float baseSpellDamage = 15f; // Base spell damage without Intelligence
    public float spellDamagePerIntelligence = 3f; // Spell damage per Intelligence

    // Movement-related stats
    public float baseMoveSpeed = 5f; // Base movement speed
    public float speedPerAgility = 0.5f; // Speed increase per Agility point
    public float jumpForce = 5f; // Force applied for jumping
    public float rotationSpeed = 10f; // Speed of character rotation

    // Core attributes
    public float strength = 5f; // Strength attribute
    public float agility = 5f; // Agility attribute
    public float intelligence = 5f; // Intelligence attribute

    private float currentHealth;
    private float currentMana;

    // Calculated properties for stats
    public float MaxHealth { get { return baseHealth + strength * healthPerStrength; } }
    public float MaxMana { get { return baseMana + intelligence * manaPerIntelligence; } }
    public float CurrentHealth { get { return currentHealth; } }
    public float CurrentMana { get { return currentMana; } }
    public float MeleeDamage { get { return baseDamage + strength * damagePerStrength; } }
    public float SpellDamage { get { return baseSpellDamage + intelligence * spellDamagePerIntelligence; } }
    public float MoveSpeed { get { return baseMoveSpeed + agility * speedPerAgility; } }

    // Initialize stats
    void Start()
    {
        currentHealth = MaxHealth;
        currentMana = MaxMana;
        Debug.Log($"Player Stats - HP: {currentHealth}/{MaxHealth}, MP: {currentMana}/{MaxMana}");
    }

    // Apply damage to player health
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, MaxHealth);
        Debug.Log($"Player health: {currentHealth}/{MaxHealth}");
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // Reduce mana by a specified amount
    public void UseMana(float amount)
    {
        currentMana -= amount;
        currentMana = Mathf.Clamp(currentMana, 0f, MaxMana);
        Debug.Log($"Player mana: {currentMana}/{MaxMana}");
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

    // Handle player death
    void Die()
    {
        Debug.Log("Player died!");
        gameObject.SetActive(false); // Disable for now
    }

    // For testing: Press 'H' to take damage, 'M' to use mana
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10f);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            UseMana(10f);
        }
    }
}