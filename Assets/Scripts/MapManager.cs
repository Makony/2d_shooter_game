using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
    }


    public Tilemap GlobalWall;
    public Tilemap GlobalFloor;



    public void GetTemplateInforomations()
    {
        GlobalWall = GlobalWallTilemap.Instance.GetComponent<Tilemap>();
        GlobalFloor = GlobalFloorTilemap.Instance.GetComponent<Tilemap>();
        foreach (Transform child in gameObject.transform)
        {
            if (!child.name.Contains("FinishingRoom"))
            {
                CombineTileMaps(child, new Vector2(child.position.x, child.position.y));

                if (!child.name.Contains("Cap") && !child.name.Contains("StartingRoom") && !child.name.Contains("Empty"))
                {
                    WaypointManager.Instance.AddWaypoint(child.transform.position + new Vector3(10f, 10f, 0f));
                }
            }

            if (child.name.Contains("FinishingRoom") && SceneManager.GetActiveScene().name == "Level2")
            {
                CombineTileMaps(child, new Vector2(child.position.x, child.position.y));
            }



            foreach (Transform grandchild in child)
            {
                if (grandchild.CompareTag("Bow"))
                {
                    BowTrapsManager.Instance.AddBow(child.gameObject);
                }

                if (grandchild.CompareTag("CCTV"))
                {
                    CCTVManager.Instance.AddCCTV(grandchild.gameObject);
                }
            }
        }
    }
    
    private void CombineTileMaps(Transform prefab, Vector2 position)
    {
        Transform gridTransform = prefab.Find("Grid");
            
            
        if (gridTransform == null)
        {
            return;
        }

        Tilemap floorTilemap = null;
        Transform floorTransform = gridTransform.Find("FloorTilemap");
        if (floorTransform != null)
        {
            floorTransform.TryGetComponent<Tilemap>(out floorTilemap);
        }

        Tilemap wallTilemap = null;
        Transform wallTransform = gridTransform.Find("WallTilemap");
        if (wallTransform != null)
        {
            wallTransform.TryGetComponent<Tilemap>(out wallTilemap);
        }

        if (floorTilemap != null)
            copyTilemap(floorTilemap, GlobalFloor, position, new Vector2(20, 20));

        if (wallTilemap != null)
            copyTilemap(wallTilemap, GlobalWall, position, new Vector2(20, 20));
    }

    private void copyTilemap(Tilemap source, Tilemap destination, Vector2 position, Vector2 size)
    {
        Vector3Int offsetPosition = new Vector3Int((int)position.x, (int)position.y, 0);

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                Vector3Int tilePosition = new Vector3Int(i, j, 0);

                destination.SetTile(offsetPosition + tilePosition, source.GetTile(tilePosition));
            }
        }
    }
}
