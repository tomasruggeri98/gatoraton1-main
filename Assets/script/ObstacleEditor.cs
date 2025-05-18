// ObstacleEditor.cs
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.Events;
using Pathfinding;

public class ObstacleEditor : MonoBehaviour
{
    [Header("Tilemap & Tiles")]
    public Tilemap tilemap;
    public TileBase obstacleTile;

    [Header("Preview Highlight")]
    public Tilemap previewTilemap;
    public TileBase previewTile;

    [Header("UI Elements")]
    public CanvasGroup editUI;
    public Text instructionsText;
    public Button finishButton;             // Botón que dispara el fin de la edición

    [Header("Controls")]
    public KeyCode startChaseKey = KeyCode.Return;

    [Header("Señal al terminar")]
    public UnityEvent OnEditingFinished;

    // Límites de la cuadrícula (en celdas) — valores por defecto
    private int gridWidth = 8;
    private int gridHeight = 8;

    private bool editMode;
    private Vector3Int lastCellHighlighted;

    void Awake()
    {
        // Suscribir el botón Finish
        if (finishButton != null)
            finishButton.onClick.AddListener(() => BeginChase());
    }

    void OnEnable()
    {
        EnterEditMode();
    }

    void Update()
    {
        if (!editMode) return;
        HighlightCellUnderMouse();
        HandlePlacement();

        if (Input.GetKeyDown(startChaseKey))
            BeginChase();
    }

    void HighlightCellUnderMouse()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cell = tilemap.WorldToCell(worldPos);
        if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight)
            return;

        if (cell != lastCellHighlighted)
        {
            previewTilemap.SetTile(lastCellHighlighted, null);
            previewTilemap.SetTile(cell, previewTile);
            lastCellHighlighted = cell;
        }
    }

    void HandlePlacement()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cell = tilemap.WorldToCell(worldPos);
        if (cell.x < 0 || cell.x >= gridWidth || cell.y < 0 || cell.y >= gridHeight)
            return;

        TileBase current = tilemap.GetTile(cell);
        if (Input.GetMouseButtonDown(0) && current == null)
        {
            tilemap.SetTile(cell, obstacleTile);
        }
        else if (Input.GetMouseButtonDown(1) && current != null)
        {
            tilemap.SetTile(cell, null);
        }
        tilemap.RefreshTile(cell);
    }

    public void BeginChase()
    {
        // Salir de edición
        editMode = false;
        previewTilemap.ClearAllTiles();
        HideEditUI();
        tilemap.RefreshAllTiles();
        AstarPath.active.Scan();

        // Disparar la señal para que el Initializer instancie y arranque la persecución
        OnEditingFinished?.Invoke();
    }

    void EnterEditMode()
    {
        editMode = true;
        lastCellHighlighted = new Vector3Int(int.MinValue, int.MinValue, 0);
        previewTilemap.ClearAllTiles();
        ShowEditUI();
    }

    void ShowEditUI()
    {
        if (editUI)
        {
            editUI.alpha = 1f;
            editUI.blocksRaycasts = true;
        }
        if (instructionsText != null)
        {
            instructionsText.text =
                $"Place walls within a {gridWidth}×{gridHeight} grid.\n" +
                "LMB: Place   RMB: Erase   Press Enter or Finish to start chase";
        }
    }

    void HideEditUI()
    {
        if (editUI)
        {
            editUI.alpha = 0f;
            editUI.blocksRaycasts = false;
        }
    }

    /// <summary>
    /// Llamar desde GameInitializer para fijar el tamaño de la cuadrícula.
    /// </summary>
    public void SetGridBounds(int width, int height)
    {
        gridWidth = width;
        gridHeight = height;
        if (editMode) ShowEditUI();
    }
}
