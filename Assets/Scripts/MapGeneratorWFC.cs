using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class MapGeneratorWFC : MonoBehaviour
{
    public const int TILE_WIDTH = 20;
    public const int TILE_HEIGHT = 20;
    public static int GRID_WIDTH = 8;
    public static int GRID_HEIGHT = 8;
    public enum TDirection
    {
        North,
        East,
        South,
        West
    }

    public enum TWall
    {
        WWW,
        WDW,
        WDD,
        DDW,
        DDD,
    }

    public static readonly HashSet<TWall> passable = new HashSet<TWall>
    {
        TWall.WDW,
        TWall.WDD,
        TWall.DDW,
        TWall.DDD
    };

    public class Tile
    {
        public Dictionary<TDirection, TWall> walls;

        public Tile(TWall north, TWall east, TWall south, TWall west)
        {
            walls = new Dictionary<TDirection, TWall>
            {
                { TDirection.North, north },
                { TDirection.East, east },
                { TDirection.South, south },
                { TDirection.West, west }
            };
        }

        public Tile(string a, string b, string c)
        {
            Dictionary<string, TWall> strToWall = new Dictionary<string, TWall>
            {
                { "###", TWall.WWW },
                { "# #", TWall.WDW },
                { "#  ", TWall.WDD },
                { "  #", TWall.DDW },
                { "   ", TWall.DDD },
            };
            walls = new Dictionary<TDirection, TWall>
            {
                { TDirection.North, strToWall[a] },
                { TDirection.East, strToWall[$"{a[2]}{b[2]}{c[2]}"] },
                { TDirection.South, strToWall[c] },
                { TDirection.West, strToWall[$"{a[0]}{b[0]}{c[0]}"] }
            };
        }
    }

    public static Dictionary<string, Tile> tiles = new Dictionary<string, Tile>
    {
        
        //Rooms
        {
            "StartingRoom", new Tile(
                "# #",
                "   ",
                "# #"
            )
        },
        {
            "FinishingRoom", new Tile(
                "# #",
                "   ",
                "# #"
            )
        },
        


    
        //PATHS
        {
            "CrossingPath", new Tile(
                "# #",
                "   ",
                "# #"
            )
        },
        {
            "CrossingPathNorth", new Tile(
                "###",
                "   ",
                "# #"
            )
        },
        {
            "CrossingPathEast", new Tile(
                "# #",
                "  #",
                "# #"
            )
        },
        {
            "CrossingPathSouth", new Tile(
                "# #",
                "   ",
                "###"
            )
        },
        {
            "CrossingPathWest", new Tile(
                "# #",
                "#  ",
                "# #"
            )
        },
        {
            "HorizontalPath", new Tile(
                "###",
                "   ",
                "###"
            )
        },
        {
            "VerticalPath", new Tile(
                "# #",
                "# #",
                "# #"
            )
        },
        {
            "90DegreePathNE", new Tile(
                "# #",
                "#  ",
                "###"
            )
        },
        {
            "90DegreePathES", new Tile(
                "###",
                "#  ",
                "# #"
            )
        },
        {
            "90DegreePathSW", new Tile(
                "###",
                "  #",
                "# #"
            )
        },
        {
            "90DegreePathWN", new Tile(
                "# #",
                "  #",
                "###"
            )
        },

        {
            "Empty", new Tile(
                "###",
                "###",
                "###"
            )
        },
        //DEADENDS
        {
            "NorthCap", new Tile(
                "###",
                "# #",
                "# #"
            )
        },
        {
            "EastCap", new Tile(
                "###",
                "  #",
                "###"
            )
        },
        {
            "SouthCap", new Tile(
                "# #",
                "# #",
                "###"
            )
        },
        {
            "WestCap", new Tile(
                "###",
                "#  ",
                "###"
            )
        },

        // Big rooms
        {
            "BigRoomNW", new Tile(
                "###",
                "#  ",
                "#  "
            )
        },
        {
            "BigRoomNE", new Tile(
                "###",
                "  #",
                "  #"
            )
        },
        {
            "BigRoomSW", new Tile(
                "#  ",
                "#  ",
                "###"
            )
        },
        {
            "BigRoomSE", new Tile(
                "  #",
                "  #",
                "###"
            )
        },
        {
            "BigRoomDoorNEE", new Tile(
                "###",
                "   ",
                "  #"
            )
        },
        {
            "BigRoomDoorSWW", new Tile(
                "#  ",
                "   ",
                "###"
            )
        },
        {
            "BigRoomN", new Tile(
                "###",
                "   ",
                "   "
            )
        },
        {
            "BigRoomE", new Tile(
                "  #",
                "  #",
                "  #"
            )
        },
        {
            "BigRoomS", new Tile(
                "   ",
                "   ",
                "###"
            )
        },
        {
            "BigRoomW", new Tile(
                "#  ",
                "#  ",
                "#  "
            )
        },
        {
            "BigRoomCenter", new Tile(
                "   ",
                "   ",
                "   "
            )
        },
        {
            "BigRoomInnerNE", new Tile(
                "  #",
                "   ",
                "   "
            )
        },
        {
            "BigRoomInnerSE", new Tile(
                "   ",
                "   ",
                "  #"
            )
        },
        {
            "BigRoomInnerSW", new Tile(
                "   ",
                "   ",
                "#  "
            )
        },
        {
            "BigRoomInnerNW", new Tile(
                "#  ",
                "   ",
                "   "
            )
        },

    };

    public static Dictionary<string, int> weights = new Dictionary<string, int> {
        { "StartingRoom", 0 },
        { "FinishingRoom", 0 },

        { "CrossingPath", 2 },
        { "CrossingPathNorth", 5 },
        { "CrossingPathEast", 5 },
        { "CrossingPathSouth", 5 },
        { "CrossingPathWest", 5 },
        { "HorizontalPath", 10 },
        { "VerticalPath", 10 },
        { "90DegreePathNE", 10 },
        { "90DegreePathES", 10 },
        { "90DegreePathSW", 10 },
        { "90DegreePathWN", 10 },

        { "Empty", 10 },

        { "NorthCap", 9 },
        { "EastCap", 9 },
        { "SouthCap", 9 },
        { "WestCap", 9 },

        { "BigRoomNW", 2 },
        { "BigRoomNE", 2 },
        { "BigRoomSW", 2 },
        { "BigRoomSE", 2 },
        { "BigRoomDoorNEE", 2 },
        { "BigRoomDoorSWW", 2 },
        { "BigRoomN", 2 },
        { "BigRoomE", 2 },
        { "BigRoomS", 2 },
        { "BigRoomW", 2 },
        { "BigRoomCenter", 2 },
        { "BigRoomInnerNE", 2 },
        { "BigRoomInnerSE", 2 },
        { "BigRoomInnerSW", 2 },
        { "BigRoomInnerNW", 2 },
    };

    public class Slot
    {
        public bool collapsed = false;

        public bool reachable = false;

        public HashSet<string> possibilities = new HashSet<string>(tiles.Keys);

        public Slot()
        {
            possibilities.Remove("StartingRoom");
            possibilities.Remove("FinishingRoom"); 
        }
        
        public Dictionary<TDirection, HashSet<TWall>> Constrain(TDirection direction, HashSet<TWall> walls)
        {
            bool changed = false;
            HashSet<TWall> northWalls = new HashSet<TWall>();
            HashSet<TWall> eastWalls = new HashSet<TWall>();
            HashSet<TWall> southWalls = new HashSet<TWall>();
            HashSet<TWall> westWalls = new HashSet<TWall>();

            string[] possibilitiesArray = possibilities.ToArray();

            for (int i = possibilities.Count - 1; i >= 0; i--)
            {
                string tile = possibilitiesArray[i];
                Tile t = tiles[tile];
                if (!walls.Contains(t.walls[direction]))
                {
                    possibilities.Remove(tile);
                    changed = true;
                }
                else
                {
                    northWalls.Add(t.walls[TDirection.North]);
                    eastWalls.Add(t.walls[TDirection.East]);
                    southWalls.Add(t.walls[TDirection.South]);
                    westWalls.Add(t.walls[TDirection.West]);
                }
            }

            if (changed)
            {
                if (possibilities.Count == 1)
                    collapsed = true;
                else if (possibilities.Count == 0)
                {
                    // unsolvable
                    Debug.LogError("Unsolvable");
                    return new Dictionary<TDirection, HashSet<TWall>>();
                }

                return new Dictionary<TDirection, HashSet<TWall>>
                {
                    { TDirection.North, northWalls },
                    { TDirection.East, eastWalls },
                    { TDirection.South, southWalls },
                    { TDirection.West, westWalls }
                };
            }
            else
                return new Dictionary<TDirection, HashSet<TWall>>();
        }

        public Dictionary<TDirection, HashSet<TWall>> Set(string tile, bool isReachable = true)
        {
            collapsed = true;
            reachable = isReachable;
            possibilities = new HashSet<string> { tile };

            return new Dictionary<TDirection, HashSet<TWall>>
            {
                { TDirection.North, new HashSet<TWall> { tiles[tile].walls[TDirection.North] } },
                { TDirection.East, new HashSet<TWall> { tiles[tile].walls[TDirection.East] } },
                { TDirection.South, new HashSet<TWall> { tiles[tile].walls[TDirection.South] } },
                { TDirection.West, new HashSet<TWall> { tiles[tile].walls[TDirection.West] } }
            };
        }

    }
  
    public Slot[,] grid = new Slot[GRID_WIDTH, GRID_HEIGHT];

    private (int x, int y) lastCapPlaced;
    
    private string lastCapPlacedName;

    private (int x, int y) NextSlot()
    {
        (int x, int y) minSlot = (-1, -1);
        int min = int.MaxValue;

        for (int x = 0; x < GRID_WIDTH; x++)
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                Slot s = grid[x, y];
                if (!s.collapsed && s.possibilities.Count < min && s.reachable)
                {
                    min = s.possibilities.Count;
                    minSlot = (x, y);
                }
            }
        }

        if (minSlot.x == -1)
            grid[lastCapPlaced.x, lastCapPlaced.y].Set("FinishingRoom");

        return minSlot;
    }

    private void Collapse(int x, int y, TDirection d, HashSet<TWall> walls)
    
    {
        if (x < 0 || x >= GRID_WIDTH || y < 0 || y >= GRID_HEIGHT)
            return;

        Slot s = grid[x, y];
        if (s.collapsed)
            return;

        Dictionary<TDirection, HashSet<TWall>> possibleWalls = s.Constrain(d, walls);
        if (possibleWalls.Count == 0)
            return;

        if (!s.reachable && (
            (y < GRID_HEIGHT -1 && grid[x, y+1].reachable && possibleWalls[TDirection.North].All(w => passable.Contains(w))) ||
            (x < GRID_WIDTH -1 && grid[x+1, y].reachable && possibleWalls[TDirection.East].All(w => passable.Contains(w))) ||
            (y > 0 && grid[x, y-1].reachable && possibleWalls[TDirection.South].All(w => passable.Contains(w))) ||
            (x > 0 && grid[x-1, y].reachable && possibleWalls[TDirection.West].All(w => passable.Contains(w)))))
            s.reachable = true;

        Collapse(x, y+1, TDirection.South, possibleWalls[TDirection.North]);
        Collapse(x+1, y, TDirection.West, possibleWalls[TDirection.East]);
        Collapse(x, y-1, TDirection.North, possibleWalls[TDirection.South]);
        Collapse(x-1, y, TDirection.East, possibleWalls[TDirection.West]);
    }

    private void WFC() 
    {
        while (true)
        {
            (int x, int y) = NextSlot();
            if (x == -1)
                break;

            Slot n = grid[x, y];

            string tile = null;
            string[] possibleTiles = n.possibilities.ToArray();
            int totalWeight = possibleTiles.Sum(t => weights[t]);
            int r = Random.Range(0, totalWeight);
            int s = 0;
            foreach (var t in possibleTiles)
            {
                s += weights[t];
                if (r <= s)
                {
                    tile = t;
                    break;
                }
            }

            Dictionary<TDirection, HashSet<TWall>> walls = n.Set(tile);

            if (tile.Contains("Cap"))
            {
                lastCapPlaced = (x, y);
                lastCapPlacedName = (tile);
            }
            
            Collapse(x, y+1, TDirection.South, walls[TDirection.North]);
            Collapse(x+1, y, TDirection.West, walls[TDirection.East]);
            Collapse(x, y-1, TDirection.North, walls[TDirection.South]);
            Collapse(x-1, y, TDirection.East, walls[TDirection.West]);
        }
    }

    public void StartWFC()
    {
        for (int x = 0; x < GRID_WIDTH; x++)
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                grid[x, y] = new Slot();

                if (x == 0) grid[x, y].Constrain(TDirection.West, new HashSet<TWall> { TWall.WWW });
                if (y == 0) grid[x, y].Constrain(TDirection.South, new HashSet<TWall> { TWall.WWW });
                if (x == GRID_WIDTH - 1) grid[x, y].Constrain(TDirection.East, new HashSet<TWall> { TWall.WWW });
                if (y == GRID_HEIGHT - 1) grid[x, y].Constrain(TDirection.North, new HashSet<TWall> { TWall.WWW });

            }
        }

        int startX = Random.Range(1, GRID_WIDTH - 1);
        int startY = Random.Range(1, GRID_HEIGHT - 1);

        Dictionary<TDirection, HashSet<TWall>> walls = grid[startX, startY].Set("StartingRoom", true);

        Collapse(startX, startY + 1, TDirection.South, walls[TDirection.North]);
        Collapse(startX + 1, startY, TDirection.West, walls[TDirection.East]);
        Collapse(startX, startY - 1, TDirection.North, walls[TDirection.South]);
        Collapse(startX - 1, startY, TDirection.East, walls[TDirection.West]);

        WFC();

        GameObject[] tilePrefabs = Resources.LoadAll<GameObject>("Prefabs/Tiles");

        for (int x = 0; x < GRID_WIDTH; x++)
        {
            for (int y = 0; y < GRID_HEIGHT; y++)
            {
                Slot s = grid[x, y];
                if (s.collapsed)
                {
                    string tileName = s.possibilities.First();
                    GameObject prefab = tilePrefabs.FirstOrDefault(p => p.name == tileName);
                    if (prefab != null)
                    {
                        Vector2 pos = new Vector2(x * TILE_WIDTH, y * TILE_HEIGHT);
                        Instantiate(prefab, pos, Quaternion.identity, mapManagerTransform);
                    }
                }
            }
        }
        LevelManager.Instance.doorDirection = lastCapPlacedName; 
    }

    public Transform mapManagerTransform;
    void Start()
    {
        if (mapManagerTransform == null)
        {
            mapManagerTransform = GameObject.Find("MapManager").transform;
        }
        StartWFC();

        if (lastCapPlacedName == null)
        {
            Debug.LogWarning("No cap tile to replace with finishingroom. Restarting :U");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        LevelManager.Instance.StartLevel1();
    }
}   