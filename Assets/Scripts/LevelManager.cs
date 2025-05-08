using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// class for managing the level, including enemy count and rewards after killing all enemies
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private float totalEnemies;
    private float remainingEnemies;
    private float enemiesKilled;
    private float LevelNumber = 0;
    [SerializeField] private GameObject rewardScreen;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject statScreen;

    private GameObject player;
    private Player playerStats;
    private PlayerAttack playerAttackStats;

    //to update statScreen
    private TextMeshProUGUI lifetext;
    private TextMeshProUGUI hptext;
    private Image healthBar;
    private TextMeshProUGUI ammotext;
    private TextMeshProUGUI enemytext;
    private TextMeshProUGUI scoretext;
    private Transform ammoIcon1;
    private Transform ammoIcon2;



    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        rewardScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        player.TryGetComponent<Player>(out playerStats);
        player.TryGetComponent<PlayerAttack>(out playerAttackStats);

        Transform EnemyManager = transform.Find("EnemyManager");
        totalEnemies = EnemyManager ? EnemyManager.childCount : 0;
        remainingEnemies = totalEnemies;
        //get stat screens
        statScreen.transform.Find("LifeText").TryGetComponent<TextMeshProUGUI>(out lifetext);
        statScreen.transform.Find("HPText").TryGetComponent<TextMeshProUGUI>(out hptext);
        statScreen.transform.Find("HealthBarGreen").TryGetComponent<Image>(out healthBar);
        statScreen.transform.Find("AmmoText").TryGetComponent<TextMeshProUGUI>(out ammotext);
        statScreen.transform.Find("EnemyText").TryGetComponent<TextMeshProUGUI>(out enemytext);
        ammoIcon1 = statScreen.transform.Find("AmmoIcon1");
        ammoIcon2 = statScreen.transform.Find("AmmoIcon2");
        gameOverScreen.transform.Find("ScoreText").TryGetComponent<TextMeshProUGUI>(out scoretext);

        UpdateAllStats();   //update them

        Debug.Log("Total enemies: " + totalEnemies);
        enemiesKilled = 0;
    }


    //For updating Stats and et.c
    public void UpdateAllStats()
    {
        LifeStat();
        HPstat();
        AmmoStat();
        EnemyStat();
    }

    public void LifeStat()
    {
        lifetext.text = (playerStats.Lifes).ToString();
    }

    public void HPstat()
    {
        float HP = Math.Clamp(playerStats.Health, 0, playerStats.MaxHP);
        hptext.text = (HP).ToString() + " / " + (playerStats.MaxHP).ToString();
        healthBar.fillAmount = HP / playerStats.MaxHP;
    }

    public void AmmoStat()
    {
        ammotext.text = (playerAttackStats.RemainingAmmo).ToString() + " / " + (playerAttackStats.MaxAmmo).ToString();
    }

    public void AmmoIcon(bool isContinuesFire)
    {
        if (isContinuesFire)
        {
            ammoIcon1.gameObject.SetActive(true);
            ammoIcon2.gameObject.SetActive(true);
        }
        else
        {
            ammoIcon1.gameObject.SetActive(false);
            ammoIcon2.gameObject.SetActive(false);
        }
    }

    public void EnemyStat()
    {
        enemytext.text = remainingEnemies.ToString();
    }


    // Method that triggers the rewardScreen when all enemies are killed
    public void EnemyKilled()
    {
        enemiesKilled++;
        remainingEnemies--;
        EnemyStat();
        //Debug.Log("after" + enemiesKilled);
        if (remainingEnemies <= 0)
        {
            Debug.Log("All enemies killed!");
            StartCoroutine(DelayRewardScreen(5f));
            //Debug.Log("Panel active in hierarchy? " + rewardScreen.activeInHierarchy);
        }
    }

    private System.Collections.IEnumerator DelayRewardScreen(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 0f; //Pause the game
        rewardScreen.SetActive(true);
    }

    // Method to handle the player's choice of reward and goes to the next level
    public void ChooseReward(string reward)
    {
        BuffPlayer();
        switch (reward)
        {
            case "Health":
                playerStats.MaxHP = (float)Math.Ceiling(playerStats.MaxHP * 2.50f);
                Debug.Log("Player MaxHP: " + playerStats.MaxHP);
                break;
            case "Ammo":
                playerAttackStats.MaxAmmo = (float)Math.Ceiling(playerAttackStats.MaxAmmo * 2.50f);
                Debug.Log("Player MaxAmmo: " + playerAttackStats.MaxAmmo);
                break;
            case "Faster Shooting":
                playerAttackStats.BulletSpeed *= 2.50f;
                Debug.Log("Player shooting speed: " + playerAttackStats.BulletSpeed);
                break;
            case "Damage":
                playerAttackStats.BulletDamage *= 2.50f;
                break;
            case "Speed":
                playerStats.speed *= 2.50f;
                break;
        }
        rewardScreen.SetActive(false);
        Time.timeScale = 1f; //Resume the game
        LevelNumber++;
        NextLevel();
    }

    //BuffPlayer after every level
    private void BuffPlayer()
    {
        float buff = 1.075f;
        playerStats.MaxHP = (float)Math.Ceiling(playerStats.MaxHP * buff);
        playerStats.speed *= buff;
        playerStats.RotationSpeed *= buff;
        playerAttackStats.bulletCooldown /= buff;
        playerAttackStats.BulletDamage *= buff;
        playerAttackStats.BulletLifetime *= buff;
        playerAttackStats.BulletSpeed *= buff;
        playerAttackStats.MaxAmmo = (float)Math.Ceiling(playerAttackStats.MaxAmmo * buff);
        playerAttackStats.ReloadTime /= buff;
        playerAttackStats.AccuracyErrorAngle /= buff;
    }

    // MEthod that calls gameOverScreen when the player is killed
    public void PlayerKilled()
    {
        Time.timeScale = 0f;
        scoretext.text = "Score = " + enemiesKilled.ToString();
        gameOverScreen.SetActive(true);
        Debug.Log("Game Over!");
    }

    // Method for the Play Again button
    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    // Method to go the next level
    void NextLevel()
    {
        float buff = 1f + 0.20f * LevelNumber;
        totalEnemies = (float)Math.Ceiling(totalEnemies * buff);
        remainingEnemies = totalEnemies;
        EnemyManager.Instance.MakeEnemies(totalEnemies, buff);
        playerStats.Health = playerStats.MaxHP;
        UpdateAllStats();
    }
}
