using UnityEngine;

/// <summary>
/// Attach to any scene prop that should occupy a grid cell.
/// On Start, registers this GameObject as the occupant of the cell at its world position.
/// On Destroy, clears the occupant so the cell becomes free.
///
/// Requires IslandGridManager to be present in the scene.
/// The GameObject's tag determines what ToolController does with it (e.g. "Flora", "Rock").
/// </summary>
[AddComponentMenu("CosmicColony/Environment/Grid Occupant")]
public class GridOccupant : MonoBehaviour
{
    private Vector2Int _registeredCell;
    private bool _registered = false;

    void Start()
    {
        if (IslandGridManager.Instance == null)
        {
            Debug.LogWarning("[GridOccupant] IslandGridManager not found — cannot register occupant.");
            return;
        }

        _registeredCell = IslandGridManager.Instance.WorldToCell(transform.position);
        GridCell cell = IslandGridManager.Instance.GetCell(_registeredCell);
        cell.occupant = gameObject;
        _registered = true;

        Debug.Log($"[GridOccupant] '{gameObject.name}' registered at cell {_registeredCell}.");
    }

    void OnDestroy()
    {
        if (!_registered) return;
        if (IslandGridManager.Instance == null) return;

        GridCell cell = IslandGridManager.Instance.GetCell(_registeredCell);
        if (cell != null && cell.occupant == gameObject)
        {
            cell.ClearOccupant();
            Debug.Log($"[GridOccupant] '{gameObject.name}' cleared from cell {_registeredCell}.");
        }
    }
}
