using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class ObjectGenerator : MonoBehaviour
{
    public static ObjectGenerator Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }

    public Tilemap WalkableTilemap;
    public GameObject CircleTrapPrefab;
    public Transform boxManager;
    public GameObject BoxPrefab;
    public int NumberOfTraps = 0;
    public int NumberOfBoxes = 0;

    public void GenerateObjects(int numTraps, int numBoxes)
    {
        boxManager = BoxManager.Instance.transform;

        NumberOfTraps = numTraps;
        NumberOfBoxes = numBoxes;

        PlaceCircleTraps();

        // StartCoroutine(PlaceBoxesAndRegisterObstacles());
        PlaceInitialBoxes();

        PlaceKeys();
    }
    /** couldn't fix it in time 
    private IEnumerator PlaceBoxesAndRegisterObstacles()
    {
        PlaceInitialBoxes();

        yield return new WaitForSeconds(1.5f);

        foreach (Transform boxTransform in boxManager)
        {
            Collider2D boxCollider = boxTransform.GetComponent<Collider2D>();
            if (boxCollider != null)
            {
                GridManager.Instance.UpdateGridObstaclesForBounds(boxCollider.bounds, false);
            }
        }
    }
    */

    private void PlaceInitialBoxes()
    {
        BoundsInt bounds = WalkableTilemap.cellBounds;
        HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

        int attempts = 0;
        while (occupiedPositions.Count < NumberOfBoxes && attempts < 5000)
        {
            Vector3Int randomPosition = GetRandomPosition(bounds);

            if (WalkableTilemap.HasTile(randomPosition) && !occupiedPositions.Contains(randomPosition))
            {
                occupiedPositions.Add(randomPosition);
                Vector3 worldPosition = WalkableTilemap.CellToWorld(randomPosition) + new Vector3(0.5f, 0.5f, 0);


                Instantiate(BoxPrefab, worldPosition, Quaternion.identity, boxManager);
            }
            attempts++;
        }
    }


    public void PlaceCircleTraps()
    {
        BoundsInt bounds = WalkableTilemap.cellBounds;
        HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

        int attempts = 0;
        while (occupiedPositions.Count < NumberOfTraps && attempts < 5000)
        {
            Vector3Int randomPosition = GetRandomPosition(bounds);
            if (WalkableTilemap.HasTile(randomPosition) && !occupiedPositions.Contains(randomPosition))
            {
                occupiedPositions.Add(randomPosition);
                Vector3 worldPosition = WalkableTilemap.CellToWorld(randomPosition) + new Vector3(0.5f, 0.5f, 0);
                GameObject trap = Instantiate(CircleTrapPrefab, worldPosition, Quaternion.identity);
                trap.GetComponent<PressurePlate>().CalculateBowsInRange();
            }
            attempts++;
        }
    }

    Vector3Int GetRandomPosition(BoundsInt bounds)
    {
        int x = Random.Range(bounds.xMin, bounds.xMax);
        int y = Random.Range(bounds.yMin, bounds.yMax);
        return new Vector3Int(x, y, 0);
    }

    public void PlaceKeys()
    {
        GameObject[] keyPrefabs = Resources.LoadAll<GameObject>("Prefabs/Keys");

        BoundsInt bounds = WalkableTilemap.cellBounds;
        HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

        foreach (GameObject keyPrefab in keyPrefabs)
        {
            bool placed = false;
            while (!placed)
            {
                Vector3Int randomPosition = GetRandomPosition(bounds);
                if (WalkableTilemap.HasTile(randomPosition) && !occupiedPositions.Contains(randomPosition))
                {
                    occupiedPositions.Add(randomPosition);
                    Vector3 worldPosition = WalkableTilemap.CellToWorld(randomPosition) + new Vector3(0.5f, 0.5f, 0);
                    Instantiate(keyPrefab, worldPosition, Quaternion.identity);
                    placed = true;
                }
            }
        }
    }


    public void GenerateLight(float chance)
    {
        GameObject LightPrefab = Resources.Load<GameObject>("Prefabs/Light/Light");
        foreach (Transform child in MapManager.Instance.transform)
        {
            float random = UnityEngine.Random.Range(0f, 1f);
            if (random < chance)
            {
                Instantiate(LightPrefab, child.transform.position + new Vector3(10f, 10f), Quaternion.identity);
            }
        }
    }

    public void GenerateKillCode()
    {
        GameObject KillCode = Resources.Load<GameObject>("Prefabs/KillCode/KillCode");
        int randomNumber = Random.Range(0, 3);
        Instantiate(KillCode, EnemyManager.Instance.spawnpoints[randomNumber], Quaternion.identity);
    }
}