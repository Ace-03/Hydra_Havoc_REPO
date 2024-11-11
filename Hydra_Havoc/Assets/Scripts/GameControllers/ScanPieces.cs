using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanPieces : MonoBehaviour
{
    CheckersBaordScript board;

    public List<PieceScript> forcedPieces;

    private void Start()
    {
        board = gameObject.GetComponent<CheckersBaordScript>();

        forcedPieces = new List<PieceScript>();
    }

    public List<PieceScript> ScanForPossibleMove(PieceScript p, int x, int y)
    {
        forcedPieces = new List<PieceScript>();

        if (board.pieces[x, y].IsForcedToMove(board.pieces, x, y))
            forcedPieces.Add(board.pieces[x, y]);

        //Debug.Log("Counter here = " + forcedPieces.Count);
        return forcedPieces;
    }
    public List<PieceScript> ScanForPossibleMove()
    {
        forcedPieces = new List<PieceScript>();


        // Check all the pieces
        for (int i = 0; i < 8; i++) // Would have to chnage the "8" if the board size changes
            for (int j = 0; j < 8; j++)
                if (board.pieces[i, j] != null && board.pieces[i, j].isWhite == board.isWhiteTurn)
                {
                    if (board.pieces[i, j].IsForcedToMove(board.pieces, i, j))
                    {
                        forcedPieces.Add(board.pieces[i, j]);
                        forcedPieces[0].transform.GetChild(0).gameObject.SetActive(true);
                    }
                }
        return forcedPieces;
    }

}
