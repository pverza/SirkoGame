using System.Collections;
using System.Collections.Generic;
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
        numberOfTurnsAvailableForCurrentPlayer--;
        if (numberOfTurnsAvailableForCurrentPlayer <= 0) {
            currentTeamThatIsPlayng = currentTeamThatIsPlayng == PieceTeam.WHITE ? PieceTeam.BLACK : PieceTeam.WHITE;
            numberOfTurnsAvailableForCurrentPlayer = movesPerPlayer;
            Piece.resetAllPiecesStatus();
        }
        if (!ignoreTurns) { uiText.text = "Turn: " + currentTeamThatIsPlayng.ToString() + "\nMoves:" + numberOfTurnsAvailableForCurrentPlayer; }
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
        uiText.text = currentTeamThatIsPlayng.ToString() + " Player \nWins";
        isGameover = true;
    }

    //todo: I need to check if all players are blocked
}
