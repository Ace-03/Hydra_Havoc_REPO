using System;
//using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

//using Unity.VisualScripting;
//using UnityEditor.XR;
using UnityEngine;

public class CheckersBaordScript : MonoBehaviour
{
    public PieceScript[,] pieces = new PieceScript[8, 8];

    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;
    public GameObject headsText;
    public GameObject tailsText;
    public GameObject whiteText;
    public GameObject blackText;
    public GameObject whiteWin;
    public GameObject blackWin;

    private Vector3 boardOffset = new Vector3(-4f, -0.7f, -4f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0, .5f);

    public bool isWhite; //might not need this variable
    private bool isWhiteTurn;
    public  bool hasChallenged;
    private bool hasWinner;

    private PieceScript selectedPiece;
    private List<PieceScript> forcedPieces;

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    // Start is called before the first frame update
    void Start()
    {
        isWhiteTurn = true;
        forcedPieces = new List<PieceScript>();
        GenerateBaord();
    }

    private void Update()
    {
        UpdateMouseOver();
        CheckVictory();
        //Debug.Log(mouseOver);

        if ((isWhite) ? isWhiteTurn : !isWhiteTurn)
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (selectedPiece != null)
                UpdatePieceDrag(selectedPiece);

            if ((Input.GetMouseButtonDown(0)))
                SelectPiece(x, y);

            if (Input.GetMouseButtonUp(0))
                TryMove((int)startDrag.x, (int)startDrag.y, x, y);
        }
    }
    private void UpdateMouseOver() 
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25, LayerMask.GetMask("Board") ) )
        {
            mouseOver.x = (int)(hit.point.x - boardOffset.x);
            mouseOver.y = (int)(hit.point.z - boardOffset.z);
        }
        else
        {
            mouseOver.x = -1;
            mouseOver.y = -1;
        }

    }
    private void UpdatePieceDrag(PieceScript p)
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25, LayerMask.GetMask("Board")))
        {
            p.transform.position = hit.point + Vector3.up;
        }

    }



    private void SelectPiece(int x, int y)
    { 
        if(x < 0 || x > 8 || y  < 0 || y > 8)
            return;
        
        PieceScript p = pieces[x, y];
        if (p != null && p.isWhite == isWhite)
        {
            if (forcedPieces.Count == 0)
            {
                selectedPiece = p;
                startDrag = mouseOver;
                //Debug.Log(selectedPiece.name);
            }
            else
            {
                // Look for the piece in our forced pieces list
                if (forcedPieces.Find(fp => fp == p) == null)
                    return;

                selectedPiece = p;
                startDrag = mouseOver;
            }
        }
    }
    private void TryMove(int x1, int y1, int x2, int y2)
    {
        forcedPieces = ScanForPossibleMove();

        //Might remove later
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];


        if (x2 < 0 || x2 >= 8 || y2 < 0 || y2 >= 8) //Checks if out of bounds
        {
            if (selectedPiece != null)
                MovePiece(selectedPiece, x1, y1);

            startDrag = Vector2.zero;
            selectedPiece = null;
            return;
        }

        if(selectedPiece != null) 
        { 
            if(endDrag == startDrag) //Checks if piece has not moved
            { 
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }

            //Checks if piece as made a valid move
            if (selectedPiece.ValidMove(pieces, x1, y1, x2, y2))
            {
                //Check if we challenged anything
                if (MathF.Abs(x2 - x1) == 2) //if the change in the x value is great then 2, then we challanged something 
                {
                    PieceScript p = pieces[(x1 + x2) / 2, (y1 + y2) / 2]; //this puts the piece we jumped over into it's own variable

                    if (p != null)
                    {
                        //pieces[(x1 + x2) / 2, (y1 + y2) / 2] = null; old code
                        //Debug.Log("Selected = " +selectedPiece);
                        //Debug.Log("P = " + p);
                        MovePiece(selectedPiece, x1, y1);
                        pieces[x2, y2] = selectedPiece;
                        //Debug.Log("Pieces = " + pieces[7, 5]);
                        Challenge(p, pieces);
                    }
                }

                // Were we supposed to challenge a piece?
                if (forcedPieces.Count != 0 && !hasChallenged)
                {
                    MovePiece(selectedPiece, x1, y1); //This block of code gets repated a lot
                    startDrag = Vector2.zero;         //Should propably fix that later
                    selectedPiece = null;
                    return;
                }

                pieces[x2, y2] = selectedPiece;
                pieces[x1, y1] = null;
                MovePiece(selectedPiece, x2, y2);

                EndTurn();
            }
            else 
            {
                MovePiece(selectedPiece, x1, y1);
                startDrag = Vector2.zero;
                selectedPiece = null;
                return;
            }
        }
    }
    private void EndTurn()
    {
        int x = (int)endDrag.x;
        int y = (int)endDrag.y;

        //Promotes Pieces
        if(selectedPiece != null) 
        {
            if (selectedPiece.isWhite && !selectedPiece.isKing && y == 7)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.transform.Rotate(Vector3.right * 90); //change to something cooler later
            }
            else if (!selectedPiece.isWhite && !selectedPiece.isKing && y == 0)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.transform.Rotate(Vector3.right * 90); //change to something cooler later
            }
        }

        selectedPiece = null;
        startDrag = Vector2.zero;

        Debug.Log("Possible moves check: " + (ScanForPossibleMove(selectedPiece, x, y).Count) + " " + hasChallenged);
        if (ScanForPossibleMove(selectedPiece, x, y).Count != 0 && hasChallenged)
            return;

        isWhiteTurn = !isWhiteTurn;
        isWhite = !isWhite;
        hasChallenged = false;
        if (isWhiteTurn)
        {
            //Shows White turn
            whiteText.SetActive(true);
            blackText.SetActive(false);
        }
        else 
        {
            //Show Black turn
            whiteText.SetActive(false);
            blackText.SetActive(true);
        }
        
    }
    private void CheckVictory()
    {
        var ps = FindObjectsOfType<PieceScript>();
        bool hasWhite = false, hasBlack = false;
        for (int i = 0; i < ps.Length; i++)
        {
            //Debug.Log("checking for victory");
            if (ps[i].isWhite)
            {
                //Debug.Log("Check Here");
                hasWhite = true;
            }
            else
            {
                //Debug.Log("Checked Here too");
                hasBlack = true;
            }
        }

        if (!hasWhite)
        {
            Victory(false);
        }
        if (!hasBlack)
        {
            Victory(true);
        }
    }
    private void Victory(bool isWhite)
    {
        if (!hasWinner)
        {
            if (isWhite)
                whiteWin.SetActive(true);
            else
                blackWin.SetActive(true);

            hasWinner = true;
        }
        
    }
    private void Challenge(PieceScript p, PieceScript[,] piece)
    {
        //Insert coin flip animation here
        int coinValue;
        coinValue = UnityEngine.Random.Range(0, 1); //changes this back to (0,2) later
        //Debug.Log("Coin Value = " + coinValue);

        if (coinValue == 1)
        {
            //Shows heads text
            headsText.SetActive(true);
            tailsText.SetActive(false);
            hasChallenged = true;

            AddBounusPiece(piece);
            
        }
        else
        {
            //Shows tails text
            headsText.SetActive(false);
            tailsText.SetActive(true);
            //remove oppents piece
            Destroy(p.gameObject);
            p = null;
            hasChallenged = true;
        }


    }

    

    private List<PieceScript> ScanForPossibleMove(PieceScript p, int x, int y)
    {
        forcedPieces = new List<PieceScript>();

        if (pieces[x,y].IsForcedToMove(pieces, x, y))
            forcedPieces.Add(pieces[x,y]);

        Debug.Log("Counter here = " + forcedPieces.Count);
        return forcedPieces;
    }
    private List<PieceScript> ScanForPossibleMove()
    {
        forcedPieces = new List<PieceScript>();

        // Check all the pieces
        for (int i = 0; i < 8; i++) // Would have to chnage the "8" if the board size changes
            for (int j = 0; j < 8; j++)
                if (pieces[i,j] != null && pieces[i,j].isWhite == isWhiteTurn)
                    if (pieces[i,j].IsForcedToMove(pieces, i , j))
                        forcedPieces.Add(pieces[i,j]);

        Debug.Log("Counter there = " + forcedPieces.Count);
        return forcedPieces;
    }

    private void GenerateBaord()
    { 
        //Generate White Side
        for(int y = 0; y < 2; y++) // change back to y < 3 later
        { 
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2) 
            { 
                GeneratePiece((oddRow) ? x : x+1, y);
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
    private void GeneratePiece(int x, int y)
    { 
        //Generates Pieces
        bool isPieceWhite = (y > 3) ? false : true;
        GameObject go = Instantiate((isPieceWhite) ? whitePiecePrefab : blackPiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        PieceScript p = go.GetComponent<PieceScript>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }
    private void AddBounusPiece(PieceScript[,] piece)
    {
        //check if opponets botton-most left-most space is open
        //keep moving up in space untill an open space if found
        //then GeratePiece(location of oppents first open space)
        if (!isWhiteTurn)
        {
            for (int y = 0; y < 3; y++)
            {
                bool oddRow = (y % 2 == 0);
                for (int x = 0; x < 8; x += 2)
                {
                    //Debug.Log(piece[x, y]);
                    if (piece[(oddRow) ? x : x + 1, y] == null)
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
                    if (piece[(oddRow) ? x : x + 1, y] == null)
                    {
                        GeneratePiece((oddRow) ? x : x + 1, y);
                        return;
                    }
                }
            }
        }
    }
    private void MovePiece(PieceScript p, int x, int y)
    { 
        //Movies pieces and keeps it in the center of it's square
        p.transform.position = (Vector3.right * x * 1f) + (Vector3.up) + (Vector3.forward * y * 1f) + boardOffset + pieceOffset;
    }

    
}
