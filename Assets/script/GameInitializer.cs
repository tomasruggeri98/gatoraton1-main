using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;

public class GameInitializer : MonoBehaviour
{
    [Header("Editor de Obstáculos")]
    public ObstacleEditor obstacleEditor;
    public Tilemap groundTilemap;
    public Camera gameCamera;

    [Header("Personajes")]
    public GameObject catPrefab;
    public GameObject mousePrefab;
    public Transform charactersParent;

    [Header("A* y Cámara")]
    public float tileSize = 1f;
    public float cameraMargin = 1f;

    private GameObject catInstance;
    private GameObject mouseInstance;

    void Start()
    {
        int w = GameSettings.GridWidth;
        int h = GameSettings.GridHeight;

        // Reset Tilemap offset
        obstacleEditor.tilemap.transform.position = Vector3.zero;

        // Alinear celdas
        obstacleEditor.tilemap.layoutGrid.cellSize = new Vector3(tileSize, tileSize, 0);

        // Configurar cuadrícula
        obstacleEditor.SetGridBounds(w, h);

        // Construir el borde de obstáculos
        BuildBorders(w, h);

        // Instanciar personajes
        PlaceCharacters(w, h);

        // Ajustar cámara y grafo A*
        ConfigureCamera(w, h);
        ConfigureGridGraph(w, h);

        // Al terminar la edición, iniciar la persecución
        obstacleEditor.OnEditingFinished.AddListener(() =>
        {
            var chase = catInstance.GetComponent<CatGridChase>();
            if (chase != null) chase.BeginChase();
        });
    }

    void BuildBorders(int w, int h)
    {
        Tilemap tilemap = obstacleEditor.tilemap;
        TileBase tile = obstacleEditor.obstacleTile;

        // Bordes horizontales
        for (int x = 0; x < w; x++)
        {
            tilemap.SetTile(new Vector3Int(x, 0, 0), tile);
            tilemap.SetTile(new Vector3Int(x, h - 1, 0), tile);
        }

        // Bordes verticales
        for (int y = 0; y < h; y++)
        {
            tilemap.SetTile(new Vector3Int(0, y, 0), tile);
            tilemap.SetTile(new Vector3Int(w - 1, y, 0), tile);
        }

        tilemap.RefreshAllTiles();
    }

    void PlaceCharacters(int w, int h)
    {
        Vector3Int catCell = new Vector3Int(1, 1, 0);
        Vector3Int mouseCell = new Vector3Int(w - 2, h - 2, 0);

        Vector3 catPos = CellCenterWorld(catCell);
        Vector3 mousePos = CellCenterWorld(mouseCell);

        catInstance = Instantiate(catPrefab, catPos, Quaternion.identity, charactersParent);
        mouseInstance = Instantiate(mousePrefab, mousePos, Quaternion.identity, charactersParent);

        var chase = catInstance.GetComponent<CatGridChase>();
        if (chase != null) chase.target = mouseInstance.transform;
    }

    void ConfigureCamera(int width, int height)
    {
        float cx = (width * tileSize) / 2f - tileSize / 2f;
        float cy = (height * tileSize) / 2f - tileSize / 2f;
        gameCamera.transform.position = new Vector3(cx, cy, gameCamera.transform.position.z);

        float halfH = (height * tileSize) / 2f + cameraMargin;
        float halfW = (width * tileSize) / 2f + cameraMargin;
        gameCamera.orthographicSize = Mathf.Max(halfH, halfW / gameCamera.aspect);
    }

    void ConfigureGridGraph(int width, int height)
    {
        var gg = AstarPath.active.data.gridGraph;
        gg.nodeSize = tileSize;
        gg.width = width;
        gg.depth = height;

        float worldW = width * tileSize;
        float worldH = height * tileSize;

        // El centro debe estar en el medio exacto
        gg.center = new Vector3(worldW / 2f - tileSize / 2f, 0, worldH / 2f - tileSize / 2f);

        AstarPath.active.Scan();
    }

    Vector3 CellCenterWorld(Vector3Int cell)
    {
        Vector3 wpos = groundTilemap.CellToWorld(cell);
        return wpos + new Vector3(tileSize / 2f, tileSize / 2f, 0);
    }
}
