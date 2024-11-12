using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceVictory : MonoBehaviour
{
    //public CheckersBaordScript board;

    public bool hasWinner;

    public GameObject whiteWin;
    public GameObject blackWin;


    private void Start()
    {
        //board = gameObject.GetComponent<CheckersBaordScript>();
    }

    public void CheckVictory()
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
}
