using UnityEngine;
using UnityEngine.UI;

public class UIEnemyHealthBar : MonoBehaviour
{
    public Slider healthBarSlider; // HP bar
    public Slider manaBarSlider; // MP bar
    public EnemyStats enemyStats; // Reference to EnemyStats
    public Transform target; // The enemy to follow
    public Vector3 offset = new Vector3(0, 1.5f, 0); // Offset above enemy

    void Start()
    {
        if (healthBarSlider == null)
        {
            healthBarSlider = GetComponent<Slider>();
        }
    }

    void Update()
    {
        if (enemyStats != null)
        {
            healthBarSlider.value = enemyStats.GetHealthPercentage();
            if (manaBarSlider != null)
            {
                manaBarSlider.value = enemyStats.GetManaPercentage();
            }
        }

        if (target != null)
        {
            transform.position = target.position + offset;
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}