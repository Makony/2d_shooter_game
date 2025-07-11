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
    public GameObject BoxPrefab;
    public int NumberOfTraps = 75;
    public int NumberOfBoxes = 75;

    static int startRoomCenterX = MapGeneratorWFC.startX * MapGeneratorWFC.TILE_WIDTH + MapGeneratorWFC.TILE_WIDTH / 2;
    static int startRoomCenterY = MapGeneratorWFC.startY * MapGeneratorWFC.TILE_HEIGHT + MapGeneratorWFC.TILE_HEIGHT / 2;
    public Vector3Int StartRoomCenter = new Vector3Int(startRoomCenterX, startRoomCenterY, 0);  // Set this in Inspector or by code
    public int ClearZoneSize = 6;       // Size of the clear zone (5x5)


    public void GenerateObjects()
    {
        PlaceCircleTraps();
        PlaceBoxes();
    }
    public void PlaceCircleTraps()
    {
        BoundsInt bounds = WalkableTilemap.cellBounds;
        HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

        int attempts = 0;
        while (occupiedPositions.Count < NumberOfTraps && attempts < 5000)
        {
            Vector3Int randomPosition = GetRandomPosition(bounds);

            // Check if the position is walkable and not already occupied.
            if (WalkableTilemap.HasTile(randomPosition) && !occupiedPositions.Contains(randomPosition) && !IsInsideClearZone(randomPosition))
            {
                occupiedPositions.Add(randomPosition); // Mark position as occupied.

                // Convert grid position to world position.
                Vector3 worldPosition = WalkableTilemap.CellToWorld(randomPosition) + new Vector3(0.5f, 0.5f, 0); // Center on tile.

                Instantiate(CircleTrapPrefab, worldPosition, Quaternion.identity);
            }
            attempts++;
        }
    }
    public void PlaceBoxes()
    {
        BoundsInt bounds = WalkableTilemap.cellBounds;
        HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

        int attempts = 0;
        while (occupiedPositions.Count < NumberOfBoxes && attempts < 5000)
        {
            Vector3Int randomPosition = GetRandomPosition(bounds);

            // Check if the position is walkable and not already occupied.
            if (WalkableTilemap.HasTile(randomPosition) && !occupiedPositions.Contains(randomPosition))
            {
                occupiedPositions.Add(randomPosition); // Mark position as occupied.

                // Convert grid position to world position.
                Vector3 worldPosition = WalkableTilemap.CellToWorld(randomPosition) + new Vector3(0.5f, 0.5f, 0); // Center on tile.

                Instantiate(BoxPrefab, worldPosition, Quaternion.identity);
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
    
    bool IsInsideClearZone(Vector3Int position)
    {
        int halfSize = ClearZoneSize / 2;
        return 
            position.x >= StartRoomCenter.x - halfSize &&
            position.x <= StartRoomCenter.x + halfSize &&
            position.y >= StartRoomCenter.y - halfSize &&
            position.y <= StartRoomCenter.y + halfSize;
    }

}