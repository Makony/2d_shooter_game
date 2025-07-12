using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.IO;

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
    public GameObject BoxPrefab;
    public GameObject EnemyPrefab;
    public int NumberOfTraps = 50;
    public int NumberOfBoxes = 50;
    public int numberOfEnemies = 3;
    private List<Vector3Int> cachedFreePositions;


    public void GenerateObjects()
    {
        PrecomputeFreePositions();
        PlaceCircleTraps();
        PlaceBoxes();
        PlaceEnemies();

        PathfindingManager.Instance.UpdateDynamicObstacles();
    }
    
    private void PrecomputeFreePositions()
    {
        cachedFreePositions = new List<Vector3Int>();
        BoundsInt bounds = WalkableTilemap.cellBounds;
        
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (WalkableTilemap.HasTile(pos) && 
                    !IsInsideClearZone(pos) && 
                    !IsInsideStartingRoom(pos))
                {
                    cachedFreePositions.Add(pos);
                }
            }
        }
    }

    public void PlaceCircleTraps()
    {
        // Create a working copy of positions
        List<Vector3Int> availablePositions = new List<Vector3Int>(cachedFreePositions);
        int trapsToPlace = Mathf.Min(NumberOfTraps, availablePositions.Count);
        
        for (int i = 0; i < trapsToPlace; i++)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector3Int pos = availablePositions[randomIndex];
            Vector3 worldPos = WalkableTilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0);
            
            GameObject newObj = Instantiate(CircleTrapPrefab, worldPos, Quaternion.identity);
            newObj.tag = "Trap";
            availablePositions.RemoveAt(randomIndex); // Remove from local copy only
        }
    }
    // public void PlaceCircleTraps()
    // {
    //     BoundsInt bounds = WalkableTilemap.cellBounds;
    //     HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

    //     int attempts = 0;
    //     while (occupiedPositions.Count < NumberOfTraps && attempts < 5000)
    //     {
    //         Vector3Int randomPosition = GetRandomPosition(bounds);

    //         // Check if the position is walkable and not already occupied.
    //         if (WalkableTilemap.HasTile(randomPosition) && !occupiedPositions.Contains(randomPosition) && !IsInsideClearZone(randomPosition))
    //         {
    //             occupiedPositions.Add(randomPosition); // Mark position as occupied.

    //             // Convert grid position to world position.
    //             Vector3 worldPosition = WalkableTilemap.CellToWorld(randomPosition) + new Vector3(0.5f, 0.5f, 0); // Center on tile.

    //             GameObject newObj = Instantiate(CircleTrapPrefab, worldPosition, Quaternion.identity);
    //             newObj.tag = "Trap"; // Use specific tag
    //         }
    //     }
    //         attempts++;
    // }

    public void PlaceBoxes()
    {
        // Create a working copy of positions
        List<Vector3Int> availablePositions = new List<Vector3Int>(cachedFreePositions);
        int boxesToPlace = Mathf.Min(NumberOfBoxes, availablePositions.Count);
        
        for (int i = 0; i < boxesToPlace; i++)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector3Int pos = availablePositions[randomIndex];
            Vector3 worldPos = WalkableTilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0);
            
            GameObject newObj = Instantiate(BoxPrefab, worldPos, Quaternion.identity);
            newObj.tag = "Box";
            availablePositions.RemoveAt(randomIndex); // Remove from local copy only
        }
    }

    public void PlaceEnemies()
    {
        // Create a working copy of positions
        List<Vector3Int> availablePositions = new List<Vector3Int>(cachedFreePositions);
        int enemiesToPlace = Mathf.Min(numberOfEnemies, availablePositions.Count);
        
        for (int i = 0; i < enemiesToPlace; i++)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Vector3Int pos = availablePositions[randomIndex];
            Vector3 worldPos = WalkableTilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0);
            
            Instantiate(EnemyPrefab, worldPos, Quaternion.identity);
            availablePositions.RemoveAt(randomIndex); // Remove from local copy only
        }
    }
    Vector3Int GetRandomPosition(BoundsInt bounds)
    {
        int x = Random.Range(bounds.xMin, bounds.xMax);
        int y = Random.Range(bounds.yMin, bounds.yMax);

        return new Vector3Int(x, y, 0);
    }

    bool IsInsideClearZone(Vector3Int position)
    {
        int StartRoomCenterX = MapGeneratorWFC.player_startX;
        int StartRoomCenterY = MapGeneratorWFC.player_startY;
        int halfSize = 5;
        return
            position.x >= StartRoomCenterX - halfSize &&
            position.x <= StartRoomCenterX + halfSize &&
            position.y >= StartRoomCenterY - halfSize &&
            position.y <= StartRoomCenterY + halfSize;
    }
    
    bool IsInsideStartingRoom(Vector3Int position)
    {
        int StartRoomCenterX = MapGeneratorWFC.player_startX;
        int StartRoomCenterY = MapGeneratorWFC.player_startY;
        int halfSize = 10;
        return
            position.x >= StartRoomCenterX - halfSize &&
            position.x <= StartRoomCenterX + halfSize &&
            position.y >= StartRoomCenterY - halfSize &&
            position.y <= StartRoomCenterY + halfSize;
    }

}