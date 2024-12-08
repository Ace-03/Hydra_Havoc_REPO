using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadsScript : MonoBehaviour
{
    public CheckersBaordScript board;
    public CameraController cameraController;

    public GameObject coin;

    public AudioSource Coin_SFX;
    float[] pitcheValues = new float[3] { 0.7f, 1f, 1.3f };

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

        Coin_SFX.pitch = pitcheValues[Random.Range(0, 3)];
        Coin_SFX.Play();

        cameraController.isShaking = true;
        //coin.transform.position = new Vector3(7, -0.7f, 0); //puts the coin back in it's starting position
    }
}
