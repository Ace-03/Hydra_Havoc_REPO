using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleController : MonoBehaviour
{
    public TextMeshProUGUI tutorial;

    private void Start()
    {

    }

    public void OpenTutorial()
    {
        Debug.Log("Tutorial");
        tutorial.enabled = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
