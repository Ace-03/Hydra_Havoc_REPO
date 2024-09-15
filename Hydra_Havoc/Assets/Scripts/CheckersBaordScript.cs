using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckersBaordScript : MonoBehaviour
{
    public PieceScript[,] pieces = new PieceScript[8, 8];

    public GameObject whitePiecePrefab;
    public GameObject blackPiecePrefab;

    private Vector3 boardOffset = new Vector3(-10, -0.4f, -10);
    private Vector3 pieceOffset = new Vector3(1.25f, 0, 1.25f);

    // Start is called before the first frame update
    void Start()
    {
        GenerateBaord();
    }

    private void GenerateBaord()
    { 
        for(int y = 0; y < 3; y++) 
        { 
            bool oddRow = (y % 2 == 0);
            for (int x = 0; x < 8; x += 2) 
            { 
                GeneratePiece((oddRow) ? x : x+1, y);
            }
        }
    }

    private void GeneratePiece(int x, int y)
    { 
        GameObject go = Instantiate(whitePiecePrefab) as GameObject;
        go.transform.SetParent(transform);
        PieceScript p = go.GetComponent<PieceScript>();
        pieces[x, y] = p;
        MovePiece(p, x, y);
    }

    private void MovePiece(PieceScript p, int x, int y)
    { 
        p.transform.position = (Vector3.right * x * 2.5f) + (Vector3.up) + (Vector3.forward * y * 2.5f) + boardOffset + pieceOffset;
    }
}
