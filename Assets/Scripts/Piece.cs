using System.Collections.Generic;
using UnityEngine;

public enum PieceTeam { 
    WHITE,
    BLACK
}

public class Piece : MonoBehaviour
{
    public int height;
    public bool jumps;
    public bool blocks;
    public bool move1more;
    public PieceTeam pieceTeam = PieceTeam.WHITE;

    public LayerMask layerMask;

    private bool isClicked = false;
    //private bool justClicked = false;
    private Vector3 offset;

    public Cell startingCell;
    public Cell targetCell;
    public bool isAlreadyMoved;
    public static HashSet<Piece> allPieces = new HashSet<Piece>();

    //Todo: Move the movement outside this script


    private void Awake()
    {
        allPieces.Add(this);
    }

    private void OnDestroy()
    {
        allPieces.Remove(this);
    }

    private void Start()
    {
        Cell cell = CastRayToFindMyCurrentCell();
        moveToThatCell(cell);
        if (blocks) { cell.ToggleBlocked(true, this.pieceTeam); }
    }

    public void Clicked() {
        isClicked = true;
        offset = transform.position - GetMouseWorldPosition(); // for the movement

        startingCell = CastRayToFindMyCurrentCell();
        if (startingCell != null)
        {
            startingCell.HigligtAdjacencCellsWithoutCell(true, CalculateDistence(), this);
        }

    }

    public void DeClicked() {
        isClicked = false;

        if (targetCell != null && targetCell.higlited)
        {
            Piece otherCellPiece = targetCell.getPiece();
            moveToThatCell(targetCell);
            if (otherCellPiece != null && otherCellPiece != this)
            {
                otherCellPiece.transform.position = Vector3.one * 100;
                if (otherCellPiece.blocks) { Cell.UpdateBlocked(); }
            }
            
            isAlreadyMoved = true;
            GameManager.instance.MoveToNextMove();
        }
        else
        {
            moveToThatCell(startingCell);
        }
    }

    private void Update()
    {
        if (isClicked) //for the movement
        {
            Vector3 targetPosition = GetMouseWorldPosition() + offset;
            transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        }
        UpdateCell(CastRayToFindMyCurrentCell());
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            return ray.GetPoint(rayDistance);
        }

        return Vector3.zero;
    }

    private void UpdateCell(Cell cell)
    {
        if (!isClicked) {
            return;
        }
        //Cell cell = other.gameObject.GetComponent<Cell>();
        if (cell == null) {
            return;
        }
        targetCell = cell;
    }


    private Cell CastRayToFindMyCurrentCell()
    {
        // Calculate the origin of the raycast
        Vector3 rayOrigin = transform.position + (Vector3.up * .5f);

        // Cast a ray in the specified direction
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, 2f, layerMask))
        {
            // Draw a gizmo line to visualize the raycast
            return hit.collider.gameObject.GetComponent<Cell>();
        }

        return null;
    }

    void moveToThatCell(Cell cell) {
        if (startingCell != null) { 
            startingCell.HigligtAdjacencCellsWithoutCell(false, CalculateDistence(), this);
            startingCell.removePiece();
        }
        //if (cell.getPiece() != null && cell.getPiece().blocks) {    //Todo: make it better
        //    cell.UpdateBlocked();                                   //Todo: make it better
        //}                                                           //Todo: make it better
        cell.addPiece(this);
        transform.position = new Vector3(cell.transform.position.x, transform.position.y, cell.transform.position.z);
        if (blocks)
        {
            Cell.UpdateBlocked();
        }
        //check if it wins
        WinningCell winningCell = cell.GetComponent<WinningCell>();
        if (winningCell != null && winningCell.theamThatWinsIfReachThisCell == this.pieceTeam)
        {
            GameManager.instance.GameOver();
        }
    }

    int CalculateDistence() {
        if (startingCell.HaveABlockerNear(pieceTeam))
        {
            return 0;
        }

        int distance = 1;

        if (move1more) {
            distance++;
        }

        if (startingCell.HaveATallerAlliedNear(pieceTeam, height)) {
            distance++;
        }

        return distance;
    }

    public static void resetAllPiecesStatus() {
        foreach (Piece piece in allPieces) {
            piece.isAlreadyMoved = false;
        }
    }
}
