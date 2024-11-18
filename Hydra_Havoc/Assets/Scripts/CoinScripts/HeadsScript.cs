using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadsScript : MonoBehaviour
{
    public CheckersBaordScript board;

    public GameObject coin;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Heads");
        board.heads = true;
        board.tails = false;
        board.Challenge();

        coin.transform.GetChild(0).gameObject.SetActive(false);
        coin.transform.GetChild(1).gameObject.SetActive(false);
        board.coinFlipping = false;
        board.Bounce();
        //coin.transform.position = new Vector3(7, -0.7f, 0); //puts the coin back in it's starting position
    }
}