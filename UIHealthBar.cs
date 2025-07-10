using UnityEngine;
using UnityEngine.UI;

public class UIHealthBar : MonoBehaviour
{
    public Slider healthBarSlider;
    public Slider manaBarSlider; // New mana bar
    public PlayerStats playerStats;

    void Start()
    {
        if (healthBarSlider == null)
        {
            healthBarSlider = GetComponent<Slider>();
        }
    }

    void Update()
    {
        if (playerStats != null)
        {
            healthBarSlider.value = playerStats.GetHealthPercentage();
            if (manaBarSlider != null)
            {
                manaBarSlider.value = playerStats.GetManaPercentage();
            }
        }
    }
}