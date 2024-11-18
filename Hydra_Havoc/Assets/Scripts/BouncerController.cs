using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncerController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        this.transform.position = new Vector3(0,-2.5f,0);
    }
}
