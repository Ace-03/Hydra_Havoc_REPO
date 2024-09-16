using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    public bool isWhite;
    public bool isKing;

    public bool IsForcedToMove(PieceScript[,] board, int x, int y)
    {
        if (isWhite || isKing)
        {
            //Top Left
            if (x >= 2 && y <= 5)
            {
                PieceScript p = board[x - 1, y + 1];
                //If there is a piece and it's not the same color
                if (p != null && p.isWhite != isWhite)
                {
                    //check if it's possible to land after the move
                    if (board[x - 2, y + 2] == null)
                        return true;
                }
            }

            //Top Right
            if (x <= 5 && y <= 5)
            {
                PieceScript p = board[x + 1, y + 1];
                //If there is a piece and it's not the same color
                if (p != null && p.isWhite != isWhite)
                {
                    //check if it's possible to land after the move
                    if (board[x + 2, y + 2] == null)
                        return true;
                }
            }
        }
        
        if(!isWhite || isKing)
        {
            //Bottom Left
            if (x >= 2 && y >= 2)
            {
                PieceScript p = board[x - 1, y - 1];
                //If there is a piece and it's not the same color
                if (p != null && p.isWhite != isWhite)
                {
                    //check if it's possible to land after the move
                    if (board[x - 2, y - 2] == null)
                        return true;
                }
            }

            //Bottom Right
            if (x <= 5 && y >= 2)
            {
                PieceScript p = board[x + 1, y - 1];
                //If there is a piece and it's not the same color
                if (p != null && p.isWhite != isWhite)
                {
                    //check if it's possible to land after the move
                    if (board[x + 2, y - 2] == null)
                        return true;
                }
            }
        }

        return false;
    }
    public bool ValidMove(PieceScript[,] board, int x1, int y1, int x2, int y2)
    {
        //Checks if your piece is moving onto another piece
        if (board[x2, y2] != null) 
            return false;

        int deltaMoveX = (int)Mathf.Abs(x1 - x2);
        int deltaMoveY = y2 - y1;
        if (isWhite || isKing)
        {
            if (deltaMoveX == 1) //regular jump
            {
                if (deltaMoveY == 1)
                    return true;
            }
            else if(deltaMoveX == 2) //kill jump 
            {
                if (deltaMoveY == 2)
                {
                    PieceScript p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if(p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }

        if (!isWhite || isKing)
        {
            if (deltaMoveX == 1) //regular jump
            {
                if (deltaMoveY == -1)
                    return true;
            }
            else if (deltaMoveX == 2) //kill jump 
            {
                if (deltaMoveY == -2)
                {
                    PieceScript p = board[(x1 + x2) / 2, (y1 + y2) / 2];
                    if (p != null && p.isWhite != isWhite)
                        return true;
                }
            }
        }
        return false;
    }
}
