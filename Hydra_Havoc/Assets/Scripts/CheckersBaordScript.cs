using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckersBaordScript : MonoBehaviour
{
    public PieceScript[,] pieces = new PieceScript[8, 8];

    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;

    private Vector3 boardOffset = new Vector3(-4f, -0.7f, -4f);
    private Vector3 pieceOffset = new Vector3(0.5f, 0, .5f);

    private PieceScript selectedPiece;

    private Vector2 mouseOver;
    private Vector2 startDrag;
    private Vector2 endDrag;

    // Start is called before the first frame update
    void Start()
    {
        GenerateBaord();
    }

    private void Update()
    {
        UpdateMouseOver();

        //Debug.Log(mouseOver);

        int x = (int)mouseOver.x;
        int y = (int)mouseOver.y;

        if((Input.GetMouseButtonDown(0))) 
            SelectPiece(x, y);

        if (Input.GetMouseButtonUp(0))
            TryMove((int)startDrag.x,(int)startDrag.y, x, y);
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

    private void SelectPiece(int x, int y)
    { 
        if(x < 0 || x > pieces.Length || y  < 0 || y > pieces.Length)
            return;
        
        PieceScript p = pieces[x, y];
        if (p != null)
        {
            selectedPiece = p;
            startDrag = mouseOver;
            Debug.Log(selectedPiece.name);
        }
    }

    private void TryMove(int x1, int y1, int x2, int y2)
    {
        MovePiece(selectedPiece, x2 , y2);
    }

    private void GenerateBaord()
    { 
        //Generate White Side
        for(int y = 0; y < 3; y++) 
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

    private void MovePiece(PieceScript p, int x, int y)
    { 
        //Movies pieces to there starting location
        p.transform.position = (Vector3.right * x * 1f) + (Vector3.up) + (Vector3.forward * y * 1f) + boardOffset + pieceOffset;
    }

    
}
