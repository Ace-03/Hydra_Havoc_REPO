using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TryMoving : MonoBehaviour
{
    public ScanPieces scan;
    public CheckersBaordScript board;

    public Vector2 startDrag;
    public Vector2 endDrag;

    public Vector3 boardOffset = new Vector3(-4f, -0.7f, -4f);
    public Vector3 pieceOffset = new Vector3(0.5f, 0, .5f);


    private void Start()
    {
        scan = gameObject.GetComponent<ScanPieces>();
        board = gameObject.GetComponent<CheckersBaordScript>();
    }

    public void TryMove(int x1, int y1, int x2, int y2)
    {
        scan.forcedPieces = scan.ScanForPossibleMove();

        //Might remove later
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        board.selectedPiece = board.pieces[x1, y1];

        board.mesh.enabled = true;

        if (x2 < 0 || x2 >= 8 || y2 < 0 || y2 >= 8) //Checks if out of bounds
        {
            if (board.selectedPiece != null)
                MovePiece(board.selectedPiece, x1, y1);

            startDrag = Vector2.zero;
            board.selectedPiece = null;
            return;
        }

        if (board.selectedPiece != null)
        {
            if (endDrag == startDrag) //Checks if piece has not moved
            {
                MovePiece(board.selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                board.selectedPiece = null;
                return;
            }

            //Checks if piece as made a valid move
            if (board.selectedPiece.ValidMove(board.pieces, x1, y1, x2, y2))
            {
                //Check if we challenged anything
                if (MathF.Abs(x2 - x1) == 2) //if the change in the x value is great then 2, then we challanged something 
                {
                    PieceScript p = board.pieces[(x1 + x2) / 2, (y1 + y2) / 2]; //this puts the piece we jumped over into it's own variable
                    board.PIECE = p;

                    if (p != null)
                    {
                        MovePiece(board.selectedPiece, x1, y1);
                        board.pieces[x2, y2] = board.selectedPiece;
                        board.hasChallenged = true;
                        board.flipButton.interactable = true;
                    }
                }

                MovePiece(board.selectedPiece, x2, y2);
                board.pieces[x2, y2] = board.selectedPiece;
                board.pieces[x1, y1] = null;

                board.EndTurn();
            }
            else
            {
                MovePiece(board.selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                board.selectedPiece = null;
                return;
            }
        }
    }

    public void MovePiece(PieceScript p, int x, int y)
    {
        //Movies pieces and keeps it in the center of it's square
        p.transform.position = (Vector3.right * x * 1f) + (Vector3.up) + (Vector3.forward * y * 1f) + boardOffset + pieceOffset;
    }
}
