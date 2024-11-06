using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class TitleController : MonoBehaviour
{
    public GameObject tutorial;

    public void OpenTutorial()
    {
        Debug.Log("Tutorial");
        tutorial.SetActive(true);
    }

    public void CloseTutorial()
    { 
        tutorial.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
