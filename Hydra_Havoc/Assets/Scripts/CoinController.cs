using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    public GameObject coin;
    public Rigidbody rb;

    void Start()
    {

    }



    public void Toss()
    {
        coin.transform.position = new Vector3(7, 1f, 0);

        int jumpForce = Random.Range(500, 900);
        rb.AddForce(0, jumpForce, 0);

        //int torqx = Random.Range(0, 30);
        int torqy = Random.Range(90, 300);
        rb.AddTorque(0, 0, torqy);

        this.transform.GetChild(0).gameObject.SetActive(true);
        this.transform.GetChild(1).gameObject.SetActive(true);

    }
}

