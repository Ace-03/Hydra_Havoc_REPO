using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePieces : MonoBehaviour
{
    public CheckersBaordScript board;
    public TryMoving move;

    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;

    private void Start()
    {
        board = gameObject.GetComponent<CheckersBaordScript>();
        move = gameObject.GetComponent<TryMoving>();
    }

    public void Board()
    {
        //Generate White Side
        for (int y = 0; y < 3; y++) // change back to y < 3 later
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }

        //Generate Black Side
        for (int y = 7; y > 4; y--)
        {
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2)
            {
                GeneratePiece((oddRow) ? x : x + 1, y);
            }
        }
    }
    
    public void BounusPiece(PieceScript[,] piece)
    {
        //check if opponets botton-most left-most space is open
        //keep moving up in space untill an open space if found
        //then GeratePiece(location of oppents first open space)
        if (board.isWhiteTurn)
        {
            for (int y = 0; y < 3; y++)
            {
                bool oddRow = (y % 2 == 0);
                for (int x = 0; x < 8; x += 2)
                {
                    //Debug.Log(piece[x, y]);
                    if (board.pieces[(oddRow) ? x : x + 1, y] == null)
                    {
                        GeneratePiece((oddRow) ? x : x + 1, y);
                        return;
                    }
                }
            }
        }
        else
        {
            for (int y = 7; y > 4; y--)
            {
                bool oddRow = (y % 2 == 0);
                for (int x = 0; x < 8; x += 2)
                {
                    //Debug.Log(piece[x, y]);
                    if (board.pieces[(oddRow) ? x : x + 1, y] == null)
                    {
                        GeneratePiece((oddRow) ? x : x + 1, y);
                        return;
                    }
                }
            }
        }
    }

    private void GeneratePiece(int x, int y)
    {
        //Generates Pieces
        bool isPieceWhite = (y > 3) ? false : true;
        GameObject go = Instantiate((isPieceWhite) ? whitePiecePrefab : blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        PieceScript p = go.GetComponent<PieceScript>();
        Debug.Log(move);
        board.pieces[x, y] = p;
        move.MovePiece(p, x, y);
    }
}
