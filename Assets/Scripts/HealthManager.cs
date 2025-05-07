using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    [Header("HealthBar")]
    public Image healthBar;
    private Player player;
    private Transform text;
    private TextMeshProUGUI hptext;

    void Start()
    {
        TryGetComponent<Player>(out player);
        text = healthBar.transform.parent.Find("HPText");
        if (text != null)
        {
            text.TryGetComponent<TextMeshProUGUI>(out hptext);
        }
    }

    void Update()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (player != null && healthBar != null)
        {
            float healthPercentage = player.Health / player.MaxHP;
            hptext.text = (healthPercentage*100).ToString() + "%";
            healthBar.fillAmount = healthPercentage;
        }
    }
    
}
