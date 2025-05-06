using UnityEngine;
using UnityEngine.SceneManagement;
// class for managing the level, including enemy count and rewards after killing all enemies
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    private int totalEnemies;
    private int enemiesKilled;
    [SerializeField] private GameObject rewardScreen;
    [SerializeField] private GameObject gameOverScreen;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        Debug.Log("Total enemies: " + totalEnemies);
        enemiesKilled = 0;
        rewardScreen.SetActive(false);
        gameOverScreen.SetActive(false);
    }

    // Method that triggers the rewardScreen when all enemies are killed
    public void EnemyKilled()
    {

        enemiesKilled++;
        Debug.Log("after" + enemiesKilled);
        if (enemiesKilled >= totalEnemies)
        {
            Debug.Log("All enemies killed!");
            Time.timeScale = 0f; //Pause the game
            rewardScreen.SetActive(true);
            Debug.Log("Panel active in hierarchy? " + rewardScreen.activeInHierarchy);
        }
    }

    // Method to handle the player's choice of reward and goes to the next level
    public void ChooseReward(string reward)
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) return;
        player.TryGetComponent<Player>(out Player playerStats);
        player.TryGetComponent<PlayerAttack>(out PlayerAttack playerAttackStats);

        switch (reward)
        {
            case "Health":
                playerStats.Health += 20f;
                playerStats.Health = Mathf.Clamp(playerStats.Health, 0, 100); //Stays between 0 and 100
                Debug.Log("Player health: " + playerStats.Health);
                break;
            case "Ammo":
                playerAttackStats.Ammo += 20f;
                playerAttackStats.Ammo = Mathf.Clamp(playerAttackStats.Ammo, 0, 100);
                Debug.Log("Player ammo: " + playerAttackStats.Ammo);
                break;
            case "Faster Shooting":
                playerAttackStats.BulletSpeed += 20f;
                Debug.Log("Player shooting speed: " + playerAttackStats.BulletSpeed);
                break;
        }

        rewardScreen.SetActive(false);
        Time.timeScale = 1f; //Resume the game
        NextLevel();
    }

    // MEthod that calls gameOverScreen when the player is killed
    public void PlayerKilled()
    {
        Time.timeScale = 0f;
        gameOverScreen.SetActive(true);
        Debug.Log("Game Over!");
    }

    // Method for the Play Again button !!! the logic is not done yet !!!
    public void PlayAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    // Method to go the next level !!! the logic is not done yet !!!
    void NextLevel()
    {

    }
}
