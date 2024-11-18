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
    /************************************************************************************/
    //PieceScripts--------------------------------------------------
    [Header("Piece Script Values")]
    public PieceScript[,] pieces = new PieceScript[8, 8];
    public PieceScript[,] tempPieces = new PieceScript[8, 8];
    public PieceScript PIECE;
    public PieceScript pieceLight;
    public PieceScript selectedPiece;

    //Other Scripts--------------------------------------------------
    [Header("Script References")]
    public CreatePieces create;
    public ScanPieces scan;
    public PieceVictory victory;
    public TryMoving move;
    public CoinController coin;

    //Mesh------------------------------------------------------------
    [Header("Meshes")]
    public MeshCollider mesh;

    //UI---------------------------------------------------------------
    [Header("UI")]
    public Button flipButton;
    public GameObject headsText;
    public GameObject tailsText;
    public GameObject whiteText;
    public GameObject blackText;
    public GameObject Bouncer;

    //Bools------------------------------------------------------------
    [Header("Bools")]
    public bool isWhite; //might not need this variable
    public bool isWhiteTurn;
    public bool hasChallenged;
    public bool lightOn;
    public bool heads;
    public bool tails;
    public bool coinFlipping;
    public bool forcedMoveActive = true;

    //Others-----------------------------------------------------------
    [Header("Other")]
    public float delay = 3f;
    private Vector2 mouseOver;
    /************************************************************************************/

    //private GameObject focedToMove;

    // Start is called before the first frame update
    void Start()
    {
        create = gameObject.GetComponent<CreatePieces>();
        victory = gameObject.GetComponent<PieceVictory>();
        scan = gameObject.GetComponent<ScanPieces>();
        move = gameObject.GetComponent<TryMoving>();

        isWhiteTurn = true;

        Bouncer = GameObject.Find("Bouncer");

        create.Board();
    }

    private void Update()
    {
        UpdateMouseOver();
        victory.CheckVictory();

        bool shouldBeWhitesTurn = (isWhite) ? isWhiteTurn : !isWhiteTurn;
        bool flipButtonNotInteractible = !flipButton.interactable;
        bool coinNotFlipping = !coinFlipping;

        if (/*shouldBeWhitesTurn &&*/ flipButtonNotInteractible && coinNotFlipping)
        {
            int x = (int)mouseOver.x;
            int y = (int)mouseOver.y;

            if (selectedPiece != null)
                UpdatePieceDrag(selectedPiece);

            if ((Input.GetMouseButtonDown(0)))
                SelectPiece(x, y);

            if (Input.GetMouseButtonUp(0))
                move.TryMove((int)move.startDrag.x, (int)move.startDrag.y, x, y);
        }
    }
    private void UpdateMouseOver() //could be moved to TryMoving script
    {
        if (!Camera.main)
        {
            Debug.Log("Unable to find main camera");
            return;
        }

        RaycastHit ray;
        bool rayFromMouseHitBoard = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out ray, 25, LayerMask.GetMask("Board"));
        
        if (rayFromMouseHitBoard)
        {
            mouseOver.x = (int)(ray.point.x - move.boardOffset.x);
            mouseOver.y = (int)(ray.point.z - move.boardOffset.z);
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


        RaycastHit ray;
        bool rayFromMouseHitBoard = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out ray, 25, LayerMask.GetMask("Board"));

        if (rayFromMouseHitBoard)
        {
            p.transform.position = ray.point + Vector3.up;
        }

    }

    public void Bounce()
    {
        //int bounceForce = UnityEngine.Random.Range(1000, 2000);
        Rigidbody bounceRB = Bouncer.GetComponent<Rigidbody>();
        bounceRB.AddForce(0, 5000, 0);
    }
    
    private void SelectPiece(int x, int y)
    {
        bool outsideTheBoard = x < 0 || x > 8 || y < 0 || y > 8;
        if (outsideTheBoard)
            return;
        
        PieceScript p = pieces[x, y];

        bool pieceExists = p != null;

        if (pieceExists)
        {
            bool isPieceTurn = p.isWhite == isWhite;
            if (isPieceTurn)
            {
                bool noPieceMustMove = scan.forcedPieces.Count == 0;
                bool noForcedMoves = !forcedMoveActive;

                if (noPieceMustMove || noForcedMoves)
                {
                    selectedPiece = p;
                    move.startDrag = mouseOver;
                }
                else
                {
                    if (scan.forcedPieces.Find(fp => fp == p) == null) // Look for the piece in our forced pieces list
                        return;

                    selectedPiece = p;
                    move.startDrag = mouseOver;
                }
                coin.transform.position = new Vector3(7, -0.7f, 0); //puts the coin back in it's starting position
                mesh = selectedPiece.GetComponent<MeshCollider>();
                mesh.enabled = false;

                Debug.Log("Temp Set");
                tempPieces = pieces;
            }
        }
        
    }
  
    public void EndTurn()
    {
        int x = (int)move.endDrag.x;
        int y = (int)move.endDrag.y;

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
        move.startDrag = Vector2.zero;


        for (int i = 0; i < 8; i++) // Checks entire array of pieces and turns off all there lights
            for (int j = 0; j < 8; j++)
                if (pieces[i, j] != null && pieces[i, j].isWhite == isWhiteTurn)
                {
                    CorrectPieces(pieces[i, j], i, j);

                    if (pieces[i, j].transform.GetChild(0).gameObject.active)
                    {
                        pieces[i, j].transform.GetChild(0).gameObject.SetActive(false);

                        //lightOn = false;
                    }
                }
                    
        //Debug.Log("Possible moves check: " + (ScanForPossibleMove(selectedPiece, x, y).Count) + " " + hasChallenged);
        if (scan.ScanForPossibleMove(selectedPiece, x, y).Count != 0 && hasChallenged)
            return;

        

        isWhiteTurn = !isWhiteTurn;
        isWhite = !isWhite;
        hasChallenged = false;
        //Bouncer.transform.position = new Vector3(0, -14f, 0);


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

    private void CorrectPieces(PieceScript piece, int x, int y)
    {
        if (isWhiteTurn)
        {
            piece.transform.rotation = Quaternion.Euler(270, 0, 0);
            piece.transform.position = new Vector3(tempPieces[x,y].transform.position.x, 1.5f, tempPieces[x,y].transform.position.z);
        }
        else
            piece.transform.rotation = Quaternion.Euler(270, 0, 180);



        //pieces = tempPieces;
    }

    public void Challenge()
    {
        if (heads)
        {
            //Shows heads text
            headsText.SetActive(true);
            tailsText.SetActive(false);
            create.BounusPiece(pieces);
        }
        else if(tails)
        {
            //Shows tails text
            headsText.SetActive(false);
            tailsText.SetActive(true);
            Destroy(PIECE.gameObject);//remove oppents piece
            PIECE = null;
        }
    }

    public void FlipCoin()
    {
        coin.Toss();
        


        coinFlipping = true;
        flipButton.interactable = false;
    }
}
