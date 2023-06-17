using System.Collections.Generic;
using UnityEngine;

public enum MovementCellDirection
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}

public class Cell : MonoBehaviour
{
    private static HashSet<Cell> allCells = new HashSet<Cell>();

    public Material defaultMaterial, highlitedMaterial;
    public GameObject blockedCellMesh;
    public GameObject blockedCellMesh1;

    public Dictionary<MovementCellDirection, HashSet<Cell>> adiacentCells = new Dictionary<MovementCellDirection, HashSet<Cell>>();
    private HashSet<Cell> unsortedAdiacentCells = new HashSet<Cell>();

    private const float RaycastDistance = 1f;
    private const float RaycastHeight = 0.5f;
    private const float RaycastOffset = .3f;

    [SerializeField]
    private Piece cellPiece;

    public LayerMask layerMask;

    public bool higlited = false;

    public Vector3 GetDirection(MovementCellDirection direction) {
        switch (direction)
        {
            case MovementCellDirection.UP:
                return Vector3.forward;
            case MovementCellDirection.DOWN:
                return Vector3.back;
            case MovementCellDirection.LEFT:
                return Vector3.left;
            case MovementCellDirection.RIGHT:
                return Vector3.right;
        }
        return Vector3.zero;
    }

    public void AddCell(Cell cell, MovementCellDirection direction) {
        if (!adiacentCells.ContainsKey(direction)) {
            adiacentCells.Add(direction, new HashSet<Cell>());
        }
        adiacentCells[direction].Add(cell);
    }

    public void addPiece(Piece pieceToAdd)
    {
        cellPiece = pieceToAdd;
    }

    public void removePiece()
    {
        cellPiece = null;
    }

    public Piece getPiece()
    {
        return cellPiece;
    }

    public void Awake()
    {
        allCells.Add(this);
    }

    public void OnDestroy()
    {
        allCells.Remove(this);
    }

    private void Start()
    {
        InitializeCells();
    }

    public void InitializeCells() {
        // Perform raycasts in all four directions
        CastRay(MovementCellDirection.UP, Vector3.right * RaycastOffset);
        CastRay(MovementCellDirection.UP, Vector3.left * RaycastOffset);
        CastRay(MovementCellDirection.DOWN, Vector3.right * RaycastOffset);
        CastRay(MovementCellDirection.DOWN, Vector3.left * RaycastOffset);
        CastRay(MovementCellDirection.LEFT, Vector3.zero);
        CastRay(MovementCellDirection.RIGHT, Vector3.zero);
        //Initialize Unsorted Adiacent Cells
        foreach (KeyValuePair<MovementCellDirection, HashSet<Cell>> kvp in adiacentCells)
        {
            unsortedAdiacentCells.UnionWith(kvp.Value);
        }
    }

    private void CastRay(MovementCellDirection direction, Vector3 offset)
    {
        // Calculate the origin of the raycast
        Vector3 rayOrigin = transform.position + (Vector3.up * RaycastHeight) + (GetDirection(direction) * RaycastDistance) + offset;

        // Cast a ray in the specified direction
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 1f, layerMask))
        {
            Cell hittedCell = hit.collider.gameObject.GetComponent<Cell>();
            if (hittedCell != null)
            {
                AddCell(hittedCell, direction);
            }
        }
    }

    public void HigligtAdjacencCellsWithoutCell(bool isHiglited, int steps, Piece startingPiece) {
        HashSet<Cell> excludedCells = new HashSet<Cell>();
        excludedCells.Add(this);
        HigligtAdjacencCellsWithoutCell(new HashSet<Cell>(excludedCells), isHiglited, steps, startingPiece);
    }

    private void HigligtAdjacencCellsWithoutCell(HashSet<Cell> excludedCells, bool isHiglited, int steps, Piece startingPiece)
    {
        if (steps <= 0) {
            return;
        }

        HashSet<Cell> cells = new HashSet<Cell>(unsortedAdiacentCells);
        if (excludedCells != null) {
            cells.ExceptWith(excludedCells);
            cells.UnionWith(excludedCells);
        }
        foreach (Cell cell in cells)
        {
            bool render = true;
            //if a piece is on my team  and I dont jump stop calculating 
            if (cell.cellPiece != null && cell.cellPiece.pieceTeam == startingPiece.pieceTeam && !startingPiece.jumps) {
                continue;
            }
            //if a piece is on my team but I can jump continue calculating but ignore this cell
            if (cell.cellPiece != null && cell.cellPiece.pieceTeam == startingPiece.pieceTeam && startingPiece.jumps)
            {
                excludedCells.Add(cell);
                render = false;
            }
            bool dontHaveAnAlliedNear = !cell.HaveAnAlliedNear(startingPiece);
            //if in this cell I have one enemy and I dont have allied near I can-t jump stop calculating
            if (cell.cellPiece != null && cell.cellPiece.pieceTeam != startingPiece.pieceTeam && dontHaveAnAlliedNear && !startingPiece.jumps) {
                continue;
            }
            //if in this cell I have one enemy and I dont have allied near I can-t jump continue calculating but ignore this cell
            if (cell.cellPiece != null && cell.cellPiece.pieceTeam != startingPiece.pieceTeam && dontHaveAnAlliedNear && startingPiece.jumps)
            {
                excludedCells.Add(cell);
                render = false;
            }

            if (steps > 1 && (cell.cellPiece == null || startingPiece.jumps)) {
                cell.HigligtAdjacencCellsWithoutCell(excludedCells, isHiglited, steps - 1, startingPiece); 
            }

            if (!render)
            {
                continue;
            }
            Renderer renderer = cell.GetComponent<Renderer>();
            // Check if the renderer component exists
            if (renderer != null)
            {
                // Change the material
                renderer.material = isHiglited ? highlitedMaterial : defaultMaterial;
                cell.higlited = isHiglited;
            }
        }
    }


    public void ToggleBlocked(bool active, PieceTeam team)
    {
            foreach (Cell cell in unsortedAdiacentCells)
            {
            if (team == PieceTeam.WHITE) 
            { 
                cell.blockedCellMesh.SetActive(active); 
            }
            else
            {
                cell.blockedCellMesh1.SetActive(active);
            }
            }
    }


    public bool CanIMoveIn() {
        if (gameObject.GetComponent<Renderer>().material.name == defaultMaterial.name) { //TODO: fixThis
            return false;
        }
        return true;
    }

    public bool HaveAnAlliedNear(Piece startingPiece) {
        foreach (Cell cell in unsortedAdiacentCells)
        {
            if (cell.cellPiece != null && cell.cellPiece.pieceTeam == startingPiece.pieceTeam && cell.cellPiece!= startingPiece) {
                return true;
            }
        }
        return false;
    }

    public bool HaveATallerAlliedNear(PieceTeam team, int height)
    {
        foreach (Cell cell in unsortedAdiacentCells)
        {
            if (cell.cellPiece != null && cell.cellPiece.pieceTeam == team && cell.cellPiece.height > height)
            {
                return true;
            }
        }
        return false;
    }

    public bool HaveABlockerNear(PieceTeam team)
    {
        foreach (Cell cell in unsortedAdiacentCells)
        {
            if (cell.cellPiece != null && cell.cellPiece.pieceTeam != team && cell.cellPiece.blocks)
            {
                return true;
            }
        }
        return false;
    }

    public HashSet<Cell> getListOfPieces()
    {
        HashSet<Cell> cells = new HashSet<Cell>(unsortedAdiacentCells);
        foreach (Cell cell in unsortedAdiacentCells)
        {
            if (cell.cellPiece == null)
            {
                cells.Remove(cell);
            }
        }
        return cells;
    }

    public HashSet<Cell> getListWithoutPieces()
    {
        HashSet<Cell> cells = new HashSet<Cell>(unsortedAdiacentCells);
        foreach (Cell cell in unsortedAdiacentCells)
        {
            if (cell.cellPiece != null)
            {
                cells.Remove(cell);
            }
        }
        return cells;
    }

    public static void UpdateBlocked() {
        if (allCells == null) { return; }

        HashSet<Cell> blockedCells = new HashSet<Cell>();
        foreach(Cell cell in allCells) {
            cell.blockedCellMesh.SetActive(false);
            cell.blockedCellMesh1.SetActive(false);
            if (cell.cellPiece != null && cell.cellPiece.blocks) {
                blockedCells.Add(cell);
            }
        }
        foreach (Cell cell in blockedCells) {
            cell.ToggleBlocked(true, cell.cellPiece.pieceTeam);
        }
    }

}
