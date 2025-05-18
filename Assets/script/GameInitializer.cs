// GameInitializer.cs
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameInitializer : MonoBehaviour
{
    [Header("Edici�n")]
    public ObstacleEditor obstacleEditor;  // Tu ObstacleEditor
    public Camera gameCamera;              // Main Camera
    public Tilemap groundTilemap;          // Tilemap de la cuadr�cula

    [Header("Prefabs Personajes")]
    public GameObject catPrefab;           // Prefab del gato
    public GameObject mousePrefab;         // Prefab del rat�n
    public Transform charactersParent;     // (Opcional) Padre para instanciarlos

    [Header("C�mara")]
    public float tileSize = 1f;        // Coincide con Grid.cellSize
    public float cameraMargin = 1f;        // Margen extra

    [Header("Posici�n inicial")]
    public int minDistanceCells = 5;       // Distancia m�nima entre gato y rat�n

    void Start()
    {
        int w = GameSettings.GridWidth;
        int h = GameSettings.GridHeight;

        // 1) Configurar editor y c�mara
        obstacleEditor.SetGridBounds(w, h);
        CenterAndZoomCamera(w, h);

        // 2) Esperar al fin de la edici�n
        obstacleEditor.OnEditingFinished.AddListener(SpawnAndStartChase);
    }

    public void SpawnAndStartChase()
    {
        int w = GameSettings.GridWidth;
        int h = GameSettings.GridHeight;

        // 2.1) Elegir celdas aleatorias con separaci�n
        Vector2Int catCell, mouseCell;
        do
        {
            catCell = new Vector2Int(Random.Range(0, w), Random.Range(0, h));
            mouseCell = new Vector2Int(Random.Range(0, w), Random.Range(0, h));
        }
        while (Vector2Int.Distance(catCell, mouseCell) < minDistanceCells);

        // 2.2) Convertir a posici�n mundo (centro de celda)
        Vector3 catPos = CellCenterWorld(catCell);
        Vector3 mousePos = CellCenterWorld(mouseCell);

        // 2.3) Instanciar
        GameObject catObj = Instantiate(catPrefab, catPos, Quaternion.identity, charactersParent);
        GameObject mouseObj = Instantiate(mousePrefab, mousePos, Quaternion.identity, charactersParent);

        // 2.4) Iniciar persecuci�n
        var chase = catObj.GetComponent<CatGridChase>();
        chase.target = mouseObj.transform;
        chase.BeginChase();
    }

    private Vector3 CellCenterWorld(Vector2Int cell)
    {
        Vector3 wpos = groundTilemap.CellToWorld(new Vector3Int(cell.x, cell.y, 0));
        return wpos + new Vector3(tileSize / 2f, tileSize / 2f, 0f);
    }

    private void CenterAndZoomCamera(int width, int height)
    {
        float cx = (width * tileSize) / 2f - tileSize / 2f;
        float cy = (height * tileSize) / 2f - tileSize / 2f;
        gameCamera.transform.position = new Vector3(cx, cy, gameCamera.transform.position.z);

        float halfH = (height * tileSize) / 2f + cameraMargin;
        float halfW = (width * tileSize) / 2f + cameraMargin;
        float sizeY = halfH;
        float sizeX = halfW / gameCamera.aspect;
        gameCamera.orthographicSize = Mathf.Max(sizeY, sizeX);
    }
}
