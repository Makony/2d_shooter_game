//using UnityEditor.U2D.Aseprite;
using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public static EnemyManager Instance { get; private set; }


    public GameObject enemyPrefab;
    private Enemy enemyStats;
    private EnemyAttack enemyAttackStats;

    private int waypointsCount;
    private Transform[] waypoints;
    private int currentWaypointIndex = 0;

    private int spawnpointsCount;
    private Vector3[] spawnpoints;
    private int currentSpawnpointIndex = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    void Start()
    {
        GetWaypoints();
        GetSpawnpoints();
    }

    private void GetWaypoints()
    {
        waypointsCount = WaypointManager.Instance.WaypointAmount();
        waypoints = new Transform[waypointsCount];
        for (int i = 0; i < waypointsCount; i++)
        {
            waypoints[i] = WaypointManager.Instance.WaypointAtIndex(i);
        }
    }

    private void GetSpawnpoints()
    {
        spawnpointsCount = SpawnpointManager.Instance.SpawnpointAmount();
        spawnpoints = new Vector3[spawnpointsCount];
        for (int i = 0; i < spawnpointsCount; i++)
        {
            spawnpoints[i] = SpawnpointManager.Instance.SpawnpointAtIndex(i).position;
        }
    }

    public void MakeEnemies(float totalEnemies, float buff)
    {
        StartCoroutine(MakingEnemies(totalEnemies, buff));
    }

    IEnumerator MakingEnemies(float totalEnemies, float buff)
    {
        for (int i = 0; i < totalEnemies; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnpoints[currentSpawnpointIndex], Quaternion.identity, transform);
            enemy.TryGetComponent<Enemy>(out enemyStats);
            enemy.TryGetComponent<EnemyAttack>(out enemyAttackStats);
            enemyStats.waypoints = new Transform[2];
            for (int j = 0; j < 2; j++)
            {
                enemyStats.waypoints[j] = waypoints[currentWaypointIndex];
                if (currentWaypointIndex == waypointsCount - 1)
                {
                    currentWaypointIndex = 0;
                }
                else currentWaypointIndex++;
            }
            if (currentSpawnpointIndex == spawnpointsCount - 1)
            {
                currentSpawnpointIndex = 0;
            }
            else currentSpawnpointIndex++;
            BuffEnemies(buff);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void BuffEnemies(float buff)
    {
        enemyStats.Speed *= buff;
        enemyStats.Health *= buff;
        enemyStats.HealthGenerator *= buff;
        enemyStats.rotationSpeed *= buff;
        enemyStats.ViewDistance *= buff;
        enemyStats.ShootingWaitTimeMultiplicator *= buff;
        enemyStats.MovingWaitTimeMutliplicator *= buff;
        enemyStats.ViewAngle *= buff;

        enemyAttackStats.bulletCooldown /= buff;
        enemyAttackStats.BulletSpeed *= buff;
        enemyAttackStats.BulletDamage *= buff;
        enemyAttackStats.MaxAmmo *= buff;
    }
}
