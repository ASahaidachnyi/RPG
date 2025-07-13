using UnityEngine;

// Manages player combat mechanics, including attacking enemies and playing animations
public class PlayerCombat : MonoBehaviour
{
    public float attackRange = 2f;
    public float baseAttackCooldown = 0.5f;
    public float cooldownReductionPerAgility = 0.02f;

    public Animator animator;
    public WeaponType currentWeaponType;

    private float lastAttackTime;
    private bool attackInput;
    private PlayerStats stats;
    private PlayerInputActions inputActions;

    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Attack.performed += ctx => attackInput = true;
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Start()
    {
        stats = GetComponent<PlayerStats>();
        SetWeaponAnimation(); // Встановлюємо відповідну бойову анімацію
    }

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

    void Attack()
    {
        // Анімація
        animator.SetTrigger("Attack");
        animator.SetBool("CombatIdle", true);

        // Логіка атаки
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

    public void TakeDamage(float damage)
    {
        animator.SetTrigger("CombatDamageTaken");
        // Тут можна зменшити HP, показати UI і т.д.
    }

    public void Die()
    {
        animator.SetTrigger("Death");
        // Вимкнути керування, UI тощо
    }

    public void SetWeaponAnimation()
    {
        if (animator != null)
        {
            animator.SetInteger("WeaponType", (int)currentWeaponType);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
