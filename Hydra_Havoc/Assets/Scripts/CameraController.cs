using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CheckersBaordScript _board;

    // Start is called before the first frame update
    void Start()
    {
        //_board = GetComponent<CheckersBaordScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_board.isWhiteTurn)
        {
            this.gameObject.transform.position = new Vector3(0, 8, -3.5f);
            this.gameObject.transform.rotation = new Quaternion(70, 0, 0, 100);
        }
        else if (!_board.isWhiteTurn) 
        {
            this.gameObject.transform.position = new Vector3(0, 8, 3.5f);
            this.gameObject.transform.rotation = new Quaternion(70, 0, 0, 1);
        }
    }
}
