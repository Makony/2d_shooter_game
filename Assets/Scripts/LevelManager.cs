using UnityEngine;
// class for managing the level, including enemy count and rewards after killing all enemies
public class LevelManager : MonoBehaviour
{
    private int totalEnemies;
    private int enemiesKilled;
    [SerializeField] private GameObject rewardScreen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
        enemiesKilled = 0;
        rewardScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnemyKilled(){
        enemiesKilled++;
        if (enemiesKilled >= totalEnemies){
            Time.timeScale = 0f; //Pause the game
            rewardScreen.SetActive(true);
        }
    }

    public void ChooseReward(string reward){
        ObjectStats playerStats = FindObjectOfType<ObjectStats>();
        Bullet bullet = FindObjectOfType<Bullet>();

        switch (reward){
            case "Health":
                playerStats.Health += 20f;
                playerStats.Health = Mathf.Clamp(playerStats.Health, 0, 100); //Stays between 0 and 100
                break;
            case "Ammo":
                playerStats.Ammo += 20f;
                playerStats.Ammo = Mathf.Clamp(playerStats.Ammo, 0, 100);
                break;
            case "Faster Shooting":
                bullet.IncreaseShooting(20f);
                break;
        }

        rewardScreen.SetActive(false);
        Time.timeScale = 1f; //Resume the game
        NextLevel();
    }

    void NextLevel(){

    }
}
