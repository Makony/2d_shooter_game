using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [Header("HealthBar")]
    public Image healthBar;

    public float maxHealth = 100f;
    
    void Update()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        Player player = GetComponent<Player>();
        if (player != null && healthBar != null)
        {
            float healthPercentage = player.Health / maxHealth;
            healthBar.fillAmount = healthPercentage;
        }
    }
    
}
