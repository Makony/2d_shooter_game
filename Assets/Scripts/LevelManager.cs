using System;
using System.Runtime.Serialization;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem.LowLevel;

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
    [SerializeField] private GameObject pausescreen;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject dialogScreen;


    public GameObject player;
    private Player playerStats;
    private PlayerAttack playerAttackStats;

    //to update statScreen
    private TextMeshProUGUI lifetext;
    private TextMeshProUGUI hptext;
    private Image healthBar;
    private TextMeshProUGUI ammotext;
    private TextMeshProUGUI enemytext;
    private TextMeshProUGUI scoretext;
    private Image enemyIcon;
    private Transform ammoIcon1;
    private Transform ammoIcon2;
    private TextMeshProUGUI KeyNumber;
    private TextMeshProUGUI KeyText;
    public bool IsLevelStarted = false;

    public string doorDirection;

    public float levelNumber;


    public Boolean Key1 = false;
    public Boolean Key2 = false;
    public Boolean Key3 = false;
    public Boolean Key4 = false;
    public Boolean IsDoorOpen = false;


    public AudioClip globalAlarmSound;
    private AudioSource alarmAudioSource;

    public Boolean IsWFCFinished = false;









    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        rewardScreen.SetActive(false);
        gameOverScreen.SetActive(false);

        globalAlarmSound = Resources.Load<AudioClip>("Audios/globalAlarmSound");
        alarmAudioSource = GetComponent<AudioSource>();
        if (alarmAudioSource == null) { alarmAudioSource = gameObject.AddComponent<AudioSource>(); }
    }

    public void Update()
    {
        if (!IsDoorOpen && Key1 && Key2 && Key3 && Key4)
        {
            OpenDoorForNextLevel();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pausescreen.activeInHierarchy)
            {
                pausescreen.SetActive(false);
                Time.timeScale = 1f;
            }
            else
            {
                pausescreen.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            controlsPanel.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            controlsPanel.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("toggle");
            dialogScreen.SetActive(!dialogScreen.activeSelf);
            PathDrawer.Instance.CalculatePath();
        }
    }

    public void StartLevel1()
    {
        MapManager.Instance.GetTemplateInforomations();
        GridManager.Instance.CreateGridFromTilemaps(GlobalWallTilemap.Instance.GetComponent<Tilemap>(), GlobalFloorTilemap.Instance.GetComponent<Tilemap>());

        int numChildren = MapManager.Instance.transform.childCount;
        ObjectGenerator.Instance.GenerateObjects(numChildren * 3, numChildren * 3);

        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        Vector3 roomCenter = MapManager.Instance.transform.Find("StartingRoom(Clone)").transform.position;
        roomCenter = new Vector3(roomCenter.x + 10f, roomCenter.y + 10f, 0);
        player = Instantiate(playerPrefab, roomCenter, Quaternion.identity);
        player.TryGetComponent<Player>(out playerStats);
        player.TryGetComponent<PlayerAttack>(out playerAttackStats);

        Transform EnemyManagerr = transform.Find("EnemyManager");
        totalEnemies = EnemyManagerr ? EnemyManagerr.childCount : 0;
        remainingEnemies = totalEnemies;
        //get stat screens
        statScreen.transform.Find("LifeText").TryGetComponent<TextMeshProUGUI>(out lifetext);
        statScreen.transform.Find("HPText").TryGetComponent<TextMeshProUGUI>(out hptext);
        statScreen.transform.Find("HealthBarGreen").TryGetComponent<Image>(out healthBar);
        statScreen.transform.Find("AmmoText").TryGetComponent<TextMeshProUGUI>(out ammotext);
        statScreen.transform.Find("EnemyIcon").TryGetComponent<Image>(out enemyIcon);
        enemyIcon.enabled = false;
        statScreen.transform.Find("EnemyText").TryGetComponent<TextMeshProUGUI>(out enemytext);
        enemytext.enabled = false;
        statScreen.transform.Find("NotificationText").TryGetComponent<TextMeshProUGUI>(out notificationText);
        ammoIcon1 = statScreen.transform.Find("AmmoIcon1");
        ammoIcon2 = statScreen.transform.Find("AmmoIcon2");
        gameOverScreen.transform.Find("ScoreText").TryGetComponent<TextMeshProUGUI>(out scoretext);
        statScreen.transform.Find("KeyNumber").TryGetComponent<TextMeshProUGUI>(out KeyNumber);
        statScreen.transform.Find("KeyMessage").TryGetComponent<TextMeshProUGUI>(out KeyText);
        KeyText.enabled = true;
        KeyNumber.enabled = true;


        UpdateAllStats();   //update them

        enemiesKilled = 0;
        EnemyManager.Instance.Getpoints(); // Get waypoints and spawnpoints
        //EnemyManager.Instance.MakeEnemies(1f, 1f); // Create enemies

        CameraControll.Instance.SetPlayer(player.transform); // Set the player for the camera to follow
        DialogManager.Instance.StartLevelIntro();
    }

    public void StartLevel2()
    {
        MapManager.Instance.GetTemplateInforomations();
        GridManager.Instance.CreateGridFromTilemaps(GlobalWallTilemap.Instance.GetComponent<Tilemap>(), GlobalFloorTilemap.Instance.GetComponent<Tilemap>());

        ObjectGenerator.Instance.GenerateLight(0.5f);

        GameObject playerPrefab = Resources.Load<GameObject>("Prefabs/Player/Player");
        Vector3 roomCenter = MapManager.Instance.transform.Find("StartingRoom(Clone)").transform.position;
        roomCenter = new Vector3(roomCenter.x + 10f, roomCenter.y + 10f, 0);
        player = Instantiate(playerPrefab, roomCenter, Quaternion.identity);
        player.TryGetComponent<Player>(out playerStats);
        player.TryGetComponent<PlayerAttack>(out playerAttackStats);
        LoadPlayerStats();

        Transform EnemyManagerr = transform.Find("EnemyManager");
        totalEnemies = EnemyManagerr ? EnemyManagerr.childCount : 0;
        remainingEnemies = totalEnemies;
        //get stat screens
        statScreen.transform.Find("LifeText").TryGetComponent<TextMeshProUGUI>(out lifetext);
        statScreen.transform.Find("HPText").TryGetComponent<TextMeshProUGUI>(out hptext);
        statScreen.transform.Find("HealthBarGreen").TryGetComponent<Image>(out healthBar);
        statScreen.transform.Find("AmmoText").TryGetComponent<TextMeshProUGUI>(out ammotext);
        statScreen.transform.Find("EnemyIcon").TryGetComponent<Image>(out enemyIcon);
        enemyIcon.enabled = false;
        statScreen.transform.Find("EnemyText").TryGetComponent<TextMeshProUGUI>(out enemytext);
        enemytext.enabled = false;
        statScreen.transform.Find("NotificationText").TryGetComponent<TextMeshProUGUI>(out notificationText);
        ammoIcon1 = statScreen.transform.Find("AmmoIcon1");
        ammoIcon2 = statScreen.transform.Find("AmmoIcon2");
        gameOverScreen.transform.Find("ScoreText").TryGetComponent<TextMeshProUGUI>(out scoretext);
        statScreen.transform.Find("KeyNumber").TryGetComponent<TextMeshProUGUI>(out KeyNumber);
        statScreen.transform.Find("KeyMessage").TryGetComponent<TextMeshProUGUI>(out KeyText);
        KeyText.enabled = false;
        KeyNumber.enabled = false;


        UpdateAllStats();   //update them

        enemiesKilled = 0;
        EnemyManager.Instance.Getpoints(); // Get waypoints and spawnpoints
        EnemyManager.Instance.MakeEnemies(10f, 0.9f); // Create enemies

        CameraControll.Instance.SetPlayer(player.transform); // Set the player for the camera to follow
        //DialogManager.Instance.StartLevelIntro();

        OpenDoorForNextLevel();
    }

    public void TriggerGlobalAlarm()
    {
        // If the alarm is not already playing, start it.
        if (alarmAudioSource != null && !alarmAudioSource.isPlaying)
        {
            alarmAudioSource.clip = globalAlarmSound;
            alarmAudioSource.loop = true;
            alarmAudioSource.Play();
            InvokeRepeating(nameof(SpawnAlarmEnemies), 0f, 3f);
        }
    }

    private void SpawnAlarmEnemies()
    {
        EnemyManager.Instance.SpawnEnemyRandomPlace();
    }

    //For updating Stats and et.c
    public void UpdateAllStats()
    {
        LifeStat();
        HPstat();
        AmmoStat();
        EnemyStat();
    }


    public void OpenDoorForNextLevel()
    {
        IsDoorOpen = true;
        Transform gridFinish = MapManager.Instance.transform.Find("FinishingRoom(Clone)").transform.Find("Grid");
        if (gridFinish == null)
        {
            Debug.Log("Levelmanager ups bupsi ");
            return;
        }

        foreach (Transform door in gridFinish)
        {
            if (door.name == doorDirection)
            {
                door.gameObject.SetActive(false);
            }
        }
    }
    public void Boxbuff(Vector3 position)
    {
        float randomChance = UnityEngine.Random.Range(0f, 1f);

        if (randomChance < 0.7f)
        {
            return;
        }
        else if (randomChance >= 0.7f && randomChance < 0.75f)
        {
            playerStats.MaxHP = (float)Math.Ceiling(playerStats.MaxHP * 1.015f);
            StartCoroutine(ShowNotification("Max HP Increased!", 2f));
        }
        else if (randomChance >= 0.75f && randomChance < 0.8f)
        {
            playerAttackStats.MaxAmmo = (float)Math.Ceiling(playerAttackStats.MaxAmmo * 1.025f);
            StartCoroutine(ShowNotification("Max Ammo Increased!", 2f));
        }
        else if (randomChance >= 0.8f && randomChance < 0.85f)
        {
            playerAttackStats.BulletSpeed *= 1.02f;
            StartCoroutine(ShowNotification("Bullet Speed Increased!", 2f));
        }
        else if (randomChance >= 0.85f && randomChance < 0.9f)
        {
            playerStats.Health = Mathf.Ceil(Math.Clamp(playerStats.Health + 10f, 0, playerStats.MaxHP));
            StartCoroutine(ShowNotification("Healed by 10+!", 2f));
        }
        else if (randomChance >= 0.9f && randomChance < 0.925f)
        {
            playerStats.Lifes += 1f;
            StartCoroutine(ShowNotification("Revive Increased!", 2f));
        }
        else if (randomChance >= 0.925f && randomChance <= 1f)
        {
            EnemyManager.Instance.MakeEnemy(position);
        }
        UpdateAllStats();
    }

    public void BoxEnemyDrop(Vector3 position)
    {
        float randomChance = UnityEngine.Random.Range(0f, 1f);

        if (randomChance < 0.925f)
        {
            return;
        }
        else if (randomChance >= 0.925f && randomChance <= 1f)
        {
            EnemyManager.Instance.MakeEnemy(position);
        }
        UpdateAllStats();
    }

    private System.Collections.IEnumerator ShowNotification(string message, float duration)
    {
        notificationText.text = message;
        notificationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        notificationText.gameObject.SetActive(false);
    }






    public void LifeStat()
    {
        lifetext.text = (playerStats.Lifes).ToString();
    }

    public void HPstat()
    {
        float HP = Mathf.Clamp(playerStats.Health, 0, playerStats.MaxHP);
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

    public void KeysStat()
    {
        int trueCount = (Key1 ? 1 : 0) +
                (Key2 ? 1 : 0) +
                (Key3 ? 1 : 0) +
                (Key4 ? 1 : 0);
        KeyNumber.text = (4 - trueCount).ToString();
        if (trueCount == 4)
        {
            DialogManager.Instance.ShowDialog("A little bird told me the teleport room is open now. How did you open the door without being there?! :O. \n\n Those ancient Keys must be really magical!");
        }
    }





    // Method that triggers the rewardScreen when all enemies are killed
    public void EnemyKilled()
    {
        if (SceneManager.GetActiveScene().name == "Level1") return;
        if (SceneManager.GetActiveScene().name == "Level2") return;

        enemiesKilled++;
        remainingEnemies--;
        EnemyStat();
        //Debug.Log("after" + enemiesKilled);
        if (remainingEnemies <= 0)
        {
            StartCoroutine(DelayRewardScreen(5f));
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
        float buff = 1.015f;
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Resume()
    {
        pausescreen.SetActive(false);
        Time.timeScale = 1f;
    }
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Application.Quit();
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
    
    public void SavePlayerStats()
    {
        // Mark that we have saved data.
        PlayerData.HasSavedData = true;

        PlayerData.MaxHP = playerStats.MaxHP;
        PlayerData.Speed = playerStats.speed;
        PlayerData.Lifes = playerStats.Lifes;
        PlayerData.RotationSpeed = playerStats.RotationSpeed;

        PlayerData.MaxAmmo = playerAttackStats.MaxAmmo;
        PlayerData.BulletDamage = playerAttackStats.BulletDamage;
        PlayerData.BulletSpeed = playerAttackStats.BulletSpeed;
        PlayerData.BulletLifetime = playerAttackStats.BulletLifetime;
        PlayerData.ReloadTime = playerAttackStats.ReloadTime;
        PlayerData.AccuracyErrorAngle = playerAttackStats.AccuracyErrorAngle;
        PlayerData.bulletCooldown = playerAttackStats.bulletCooldown;
    }

    public void LoadPlayerStats()
    {
        if (!PlayerData.HasSavedData) return;

        playerStats.MaxHP = PlayerData.MaxHP;
        playerStats.speed = PlayerData.Speed;
        playerStats.Lifes = PlayerData.Lifes;
        playerStats.RotationSpeed = PlayerData.RotationSpeed;

        playerAttackStats.MaxAmmo = PlayerData.MaxAmmo;
        playerAttackStats.BulletDamage = PlayerData.BulletDamage;
        playerAttackStats.BulletSpeed = PlayerData.BulletSpeed;
        playerAttackStats.BulletLifetime = PlayerData.BulletLifetime;
        playerAttackStats.ReloadTime = PlayerData.ReloadTime;
        playerAttackStats.AccuracyErrorAngle = PlayerData.AccuracyErrorAngle;
        playerAttackStats.bulletCooldown = PlayerData.bulletCooldown;

        // Reset HP so it is not that hard.
        playerStats.Health = playerStats.MaxHP;
    }
}
