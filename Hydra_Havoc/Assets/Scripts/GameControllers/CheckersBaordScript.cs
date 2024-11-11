using System;
using System.Collections;

//using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;


//using Unity.VisualScripting;
//using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;

public class CheckersBaordScript : MonoBehaviour
{
    public PieceScript[,] pieces = new PieceScript[8, 8];
    public PieceScript[,] tempPieces = new PieceScript[8, 8];
    public PieceScript PIECE;
    public PieceScript pieceLight;

    public CreatePieces create;
    public ScanPieces scan;
    public PieceVictory victory;

    public CoinController coin;

    public GameObject headsText;
    public GameObject tailsText;
    public GameObject whiteText;
    public GameObject blackText;

    public MeshCollider mesh;

    public Button flipButton;

    private Vector3 boardOffset = new Vector3(-4f, -0.7f, -4f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0, .5f);

    public bool isWhite; //might not need this variable
    public bool isWhiteTurn;
    public  bool hasChallenged;
    
    public bool lightOn;
    public bool heads;
    public bool tails;
    public bool coinFlipping;
    public bool forcedMoveActive = true;

    public float delay = 3f;

    private PieceScript selectedPiece;
    public List<PieceScript> forcedPieces;

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;


    private GameObject focedToMove;

    // Start is called before the first frame update
    void Start()
    {
        create = gameObject.GetComponent<CreatePieces>();
        victory = gameObject.GetComponent<PieceVictory>();
        scan = gameObject.GetComponent<ScanPieces>();

        isWhiteTurn = true;
        forcedPieces = new List<PieceScript>();
        create.Board();
    }

    private void Update()
    {
        UpdateMouseOver();
        victory.CheckVictory();
        //Debug.Log(mouseOver);

        if (((isWhite) ? isWhiteTurn : !isWhiteTurn) && !flipButton.interactable && !coinFlipping)
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
            if (forcedPieces.Count == 0 || !forcedMoveActive)
            {
                selectedPiece = p;
                startDrag = mouseOver;

                //MeshCollider mesh = selectedPiece.GetComponent<MeshCollider>();
                //mesh.enabled = false;

                //Debug.Log(selectedPiece.name);
            }
            else
            {
                // Look for the piece in our forced pieces list
                if (forcedPieces.Find(fp => fp == p) == null)
                    return;

                selectedPiece = p;
                startDrag = mouseOver;


                //MeshCollider mesh = selectedPiece.GetComponent<MeshCollider>();
                //mesh.enabled = false;
            }

            coin.transform.position = new Vector3(7, -0.7f, 0); //puts the coin back in it's starting position



            mesh = selectedPiece.GetComponent<MeshCollider>();
            //Debug.Log("Mesh to BRRRRRRRRRRRRR");
            mesh.enabled = false;
        }
    }
    private void TryMove(int x1, int y1, int x2, int y2)
    {
        forcedPieces = scan.ScanForPossibleMove();

        //Might remove later
        startDrag = new Vector2(x1, y1);
        endDrag = new Vector2(x2, y2);
        selectedPiece = pieces[x1, y1];

        //Rigidbody rig = selectedPiece.GetComponent<Rigidbody>();
        //rig.isKinematic = false;

        //MeshCollider mesh = selectedPiece.GetComponent<MeshCollider>();
        mesh.enabled = true;

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
                    PIECE = p;

                    if (p != null)
                    {
                        //Debug.Log("Moved Piece");
                        MovePiece(selectedPiece, x1, y1);
                        pieces[x2, y2] = selectedPiece;
                        hasChallenged = true;
                        flipButton.interactable = true;
                    }
                }

                /* This is a check to see if forced move should have happened
                 * but it seems to be uncessisary now
                if (forcedPieces.Count != 0 && !hasChallenged) // Were we supposed to challenge a piece?
                {
                    Debug.Log("Moved this piece");
                    MovePiece(selectedPiece, x1, y1); //This block of code gets repated a lot
                    startDrag = Vector2.zero;         //Should propably fix that later
                    selectedPiece = null;
                    return;
                }
                */

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
                selectedPiece.transform.localScale *= 1.5f; //change to something cooler later
            }
            else if (!selectedPiece.isWhite && !selectedPiece.isKing && y == 0)
            {
                selectedPiece.isKing = true;
                selectedPiece.transform.localScale *= 1.5f; //change to something cooler later
            }
        }

        selectedPiece = null;
        startDrag = Vector2.zero;


        for (int i = 0; i < 8; i++) // Checks entire array of pieces and turns off all there lights
            for (int j = 0; j < 8; j++)
                if (pieces[i, j] != null && pieces[i, j].isWhite == isWhiteTurn)
                    if (pieces[i,j].transform.GetChild(0).gameObject.active)
                    {
                        pieces[i,j].transform.GetChild(0).gameObject.SetActive(false);
                        //lightOn = false;
                    }
        //Debug.Log("Possible moves check: " + (ScanForPossibleMove(selectedPiece, x, y).Count) + " " + hasChallenged);
        if (scan.ScanForPossibleMove(selectedPiece, x, y).Count != 0 && hasChallenged)
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
  
    public void Challenge()//PieceScript p, PieceScript[,] piece)
    {
        if (heads)
        {
            //Shows heads text
            headsText.SetActive(true);
            tailsText.SetActive(false);
            //hasChallenged = true;
            //Debug.Log("Add piece");
            create.BounusPiece(pieces);
        }
        else if(tails)
        {
            //Shows tails text
            headsText.SetActive(false);
            tailsText.SetActive(true);
            //remove oppents piece
            //Debug.Log("Desotry Piece");
            Destroy(PIECE.gameObject);
            PIECE = null;
            //hasChallenged = true;
        }

        
    }

    public void FlipCoin()
    {
        coin.Toss();

        coinFlipping = true;
        flipButton.interactable = false;
    }

  
    public void MovePiece(PieceScript p, int x, int y)
    { 
        //Movies pieces and keeps it in the center of it's square
        p.transform.position = (Vector3.right * x * 1f) + (Vector3.up) + (Vector3.forward * y * 1f) + boardOffset + pieceOffset;
    }

    
}
