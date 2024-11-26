using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraController : MonoBehaviour
{
    public CheckersBaordScript _board;

    private Quaternion _q1 = Quaternion.Euler(0, 0, 0);
    private Quaternion _q2 = Quaternion.Euler(0, 180, 0);

    public float smoothFactor = 10f;

    public bool isShaking;

    float shakeAmount = 3f;

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

        if (isShaking)
        {
            Debug.Log("Is shaking");
            Vector3 targetPos = new Vector3(Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount), 0);
            this.gameObject.transform.position = Vector3.Lerp(transform.position, targetPos, smoothFactor * Time.deltaTime);
            StartCoroutine(Shake());
            
        } 
    }

    IEnumerator Shake()
    {
        yield return new WaitForSeconds(0.25f);
        isShaking = false;
        this.transform.position = Vector3.zero;
    }

}
