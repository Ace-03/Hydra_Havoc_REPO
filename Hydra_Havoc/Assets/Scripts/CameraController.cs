using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CheckersBaordScript _board;

    private Quaternion _q1 = Quaternion.Euler(0, 0, 0);
    private Quaternion _q2 = Quaternion.Euler(0, 180, 0);

    public float smoothFactor = 6f;

    void FixedUpdate()
    {
        if (_board.isWhiteTurn)
        {
            this.gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, _q1, smoothFactor * Time.deltaTime);
        }
        else if (!_board.isWhiteTurn) 
        {
            this.gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, _q2, smoothFactor * Time.deltaTime);
        }
    }
}
