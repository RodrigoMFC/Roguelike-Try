using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [Header("Map Settings")]
    [SerializeField] private int width = 80;
    [SerializeField] private int height = 45;
    [SerializeField] private int roomMaxSize = 10;
    [SerializeField] private int roomMinSize = 6;
    [SerializeField] private int maxRooms = 30;
    [SerializeField] private int maxMonstersPerRoom = 2;

    [Header("Tiles")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase fogTile;

    [Header("Objects")]
    [SerializeField] private TileBase crateTile;
    [SerializeField] private TileBase torchTile;
    [SerializeField] private TileBase torchBaseTile;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap fogMap;
    [SerializeField] private Tilemap objectsMap;

    [Header("Grass")]
    [SerializeField] private TileBase grassTile1;
    [SerializeField] private TileBase grassTile2;
    [SerializeField] private TileBase grassTile3;
    [SerializeField] private TileBase grassTile4;
    [SerializeField] private TileBase grassTile5;
    private TileBase[] grassTiles;

    [Header("Features")]
    [SerializeField] private List<RectangularRoom> rooms = new List<RectangularRoom>();
    [SerializeField] private List<Vector3Int> visibleTiles = new List<Vector3Int>();
    [SerializeField] private Dictionary<Vector3Int, TileData> tiles = new Dictionary<Vector3Int, TileData>();
    private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

    [Header("Entities")]
    [SerializeField] private GameObject goblin;
    [SerializeField] private GameObject imp;
    [SerializeField] private GameObject player;

    public int Width { get => width; }
    public int Height { get => height; }
    public TileBase FloorTile { get => floorTile; }
    public TileBase GrassTile1 { get => grassTile1; }
    public TileBase GrassTile2 { get => grassTile2; }
    public TileBase GrassTile3 { get => grassTile3; }
    public TileBase GrassTile4 { get => grassTile2; }
    public TileBase GrassTile5 { get => grassTile3; }
    public TileBase CrateTile { get => crateTile; }
    public TileBase TorchTile { get => torchTile; }
    public TileBase TorchBaseTile { get => torchBaseTile; }

    public Tilemap FloorMap { get => floorMap; }
    public Tilemap FogMap { get => fogMap; }
    public Tilemap ObjectsMap { get => objectsMap; }

    public Dictionary<Vector2Int, Node> Nodes { get => nodes; set => nodes = value; }

    public SpriteRenderer noiseVis;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        grassTiles = new TileBase[] { grassTile1, grassTile2, grassTile3 , grassTile4, grassTile5};

        ProcGen procGen = new ProcGen();
        procGen.GenerateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, maxMonstersPerRoom, rooms);

        AddTileMapToDictionary(floorMap);

        SetupFogMap();

    }

    ///<summary>Return True if x and y are inside of the bounds of this map. </summary>
    public bool InBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

    public void CreateEntity(string entity, Vector2 position)
    {
        switch (entity)
        {
            case "Player":
                GameObject playerObject = Instantiate(player, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity);
                playerObject.name = "Player";
                Transform playerTransform = playerObject.transform;
                CameraTracker cameraTracker = FindObjectOfType<CameraTracker>();
                cameraTracker.SetTarget(playerTransform);
                break;
            case "Imp":
                Instantiate(imp, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Imp";
                break;
            case "Goblin":
                Instantiate(goblin, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Goblin";
                break;
            case "Grass":
                Vector3Int tilePos = new Vector3Int((int)position.x, (int)position.y, 0);
                if (floorMap.HasTile(tilePos) && floorMap.GetColliderType(tilePos) == Tile.ColliderType.None)
                {
                    objectsMap.SetTile(new Vector3Int((int)position.x, (int)position.y, 0), grassTiles[Random.Range(0, grassTiles.Length)]); // Get a random index
                }
                break;
            default:
                Debug.LogError("Entity not found");
                break;
        }
    }

    public void UpdateFogMap(List<Vector3Int> playerFOV)
    {
        foreach (Vector3Int pos in tiles.Keys)
        {
            if (!tiles[pos].IsExplored)
            {
                tiles[pos].IsExplored = true;
                tiles[pos].IsVisible = false;
                fogMap.SetColor(pos, new Color(1.0f, 1.0f, 1.0f, 0.5f));
            }

            
        }

        visibleTiles.Clear();

        foreach (Vector3Int pos in playerFOV)
        {
            tiles[pos].IsVisible = true;
            fogMap.SetColor(pos, Color.clear);
            visibleTiles.Add(pos);
        }
    }

    public void SetEntitiesVisibilities()
    {
        foreach (Entity entity in GameManager.instance.Entities)
        {
            if (entity.GetComponent<Player>())
            {
                continue;
            }

            Vector3Int entityPosition = floorMap.WorldToCell(entity.transform.position);

            if (visibleTiles.Contains(entityPosition))
            {
                entity.GetComponentInChildren<SpriteRenderer>().enabled = true;
            }
            else
            {
                entity.GetComponentInChildren<SpriteRenderer>().enabled = false;
            }
        }
    }

    private void AddTileMapToDictionary(Tilemap tilemap)
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos))
            {
                continue;
            }

            TileData tile = new TileData();
            tiles.Add(pos, tile);
        }
    }

    private void SetupFogMap()
    {
        foreach (Vector3Int pos in tiles.Keys)
        {
            fogMap.SetTile(pos, fogTile);
            fogMap.SetTileFlags(pos, TileFlags.None);
        }
    }
}