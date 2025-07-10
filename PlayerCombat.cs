using UnityEngine;

// Manages player combat mechanics, including attacking enemies within range
public class PlayerCombat : MonoBehaviour
{
    public float attackRange = 2f; // Range of the player's melee attack
    public float baseAttackCooldown = 0.5f; // Base cooldown between attacks
    public float cooldownReductionPerAgility = 0.02f; // Cooldown reduction per agility point

    private float lastAttackTime; // Tracks the time of the last attack
    private bool attackInput; // Tracks if attack input was received
    private PlayerStats stats; // Reference to PlayerStats for damage calculations
    private PlayerInputActions inputActions; // Input system for attack input

    // Initialize input system and bind attack action
    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Attack.performed += ctx => attackInput = true;
    }

    // Enable input actions
    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    // Disable input actions
    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    // Initialize by getting PlayerStats component
    void Start()
    {
        stats = GetComponent<PlayerStats>();
    }

    // Handle attack input and cooldown
    void Update()
    {
        float attackCooldown = Mathf.Max(0.1f, baseAttackCooldown - stats.agility * cooldownReductionPerAgility);

        if (attackInput && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
            lastAttackTime = Time.time;
            attackInput = false;
        }
    }

    // Perform melee attack, dealing damage to enemies in range
    void Attack()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.CompareTag("Enemy"))
            {
                EnemyCombat enemyCombat = enemy.GetComponent<EnemyCombat>();
                if (enemyCombat != null)
                {
                    enemyCombat.TakeDamage(stats.MeleeDamage);
                    Debug.Log($"{gameObject.name} attacked {enemy.gameObject.name} for {stats.MeleeDamage} damage.");
                }
            }
        }
    }

    // Visualize attack range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}