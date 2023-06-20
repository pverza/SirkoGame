using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField]
    private PieceTeam currentTeamThatIsPlayng = PieceTeam.WHITE;
    public int movesPerPlayer = 2;
    private int numberOfTurnsAvailableForCurrentPlayer = 1;
    public bool ignoreTurns = false;
    public bool isGameover = false;

    public Text uiText;

    //a sort of singleton
    private void Awake()
    {
        if (GameManager.instance == null)
        {
            GameManager.instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MoveToNextMove() {
        if (isGameover) { return; };
        if (!Piece.CheckIfBothPlayersAreOnTheField()) 
        {
            GameOver(false);
            return;
        }
        if (Piece.AllAlivePiecesAreBlocked()) {
            GameOver(true);
            return;
        }
        numberOfTurnsAvailableForCurrentPlayer--;
        if (numberOfTurnsAvailableForCurrentPlayer <= 0) {
            currentTeamThatIsPlayng = currentTeamThatIsPlayng == PieceTeam.WHITE ? PieceTeam.BLACK : PieceTeam.WHITE;
            numberOfTurnsAvailableForCurrentPlayer = movesPerPlayer;
            Piece.resetAllPiecesStatus();
        }
        if (!ignoreTurns) { UpdateText("Turn: " + currentTeamThatIsPlayng.ToString() + "\nMoves:" + numberOfTurnsAvailableForCurrentPlayer); }
    }

    public bool IsTeamsTurn(PieceTeam pieceTeam) {
        if (ignoreTurns) {
            return true;
        }
        if (currentTeamThatIsPlayng == pieceTeam) {
            return true;
        }
        return false;
    }

    public void GameOver(bool isStall) {
        UpdateText(currentTeamThatIsPlayng.ToString() + " Player \nWins");
        isGameover = true;
    }


    //todo: I need to check if all players are blocked
    //very hacky way to to set the networkBehaviour.
    public List<Piece> allPieces;

    public Piece getPieceWithPieceID(int pieceId) {
        foreach (Piece p in allPieces) {
            if (p.pieceId == pieceId) {
                return p;
            }
        }
        return null;
    }

    public Cell [] cells;

    public int[] GetHiglightedStatus() {
        List<int> highlightedCells = new List<int>();
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].higlited)
            {
                highlightedCells.Add(i);
            }
        }
        return highlightedCells.ToArray();
    }

    public int[] GetBlockedPositions() {
        List<int> blockedPositions = new List<int>();
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].blockedCellMesh.activeSelf)
            {
                blockedPositions.Add(i);
            }
        }
        return blockedPositions.ToArray();
    }

    public int[] GetBlockedPositions1() {
        List<int> blockedPositions = new List<int>();
        for (int i = 0; i < cells.Length; i++)
        {
            if (cells[i].blockedCellMesh1.activeSelf)
            {
                blockedPositions.Add(i);
            }
        }
        return blockedPositions.ToArray();
    }

    public void ResetBlocked() {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].blockedCellMesh.SetActive(false);
            cells[i].blockedCellMesh1.SetActive(false);
        }
    }

    public void SetBlockedPositions(int[] positionsToBlock) {
        for (int i = 0; i < positionsToBlock.Length; i++) {
            cells[positionsToBlock[i]].blockedCellMesh.SetActive(true);
        }
    }

    public void SetBlockedPositions1(int[] positionsToBlock)
    {
        for (int i = 0; i < positionsToBlock.Length; i++)
        {
            cells[positionsToBlock[i]].blockedCellMesh1.SetActive(true);
        }
    }

    public void SetHiglightedCells(int[] positionsToHiglight)
    {
        ResetHighlightedCells();

        for (int i = 0; i < positionsToHiglight.Length; i++)
        {
            Cell cell = cells[positionsToHiglight[i]];
            Renderer renderer = cell.GetComponent<Renderer>();
            // Check if the renderer component exists
            if (renderer != null)
            {
                // Change the material
                renderer.material = cell.highlitedMaterial;
                cell.higlited = true;
            }
            
        }
    }

    public void ResetHighlightedCells() {
        foreach (Cell c in cells)
        {
            Renderer renderer = c.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material = c.defaultMaterial;
            }
        }
    }
    private void UpdateText(string text) {
        uiText.text = text;
        if (NetworkManager.Singleton.IsServer) {
            UpdateTextRpc(text);
        }
    }

    [ClientRpc]
    private void UpdateTextRpc(string text)
    {
        uiText.text = text;
    }
}
