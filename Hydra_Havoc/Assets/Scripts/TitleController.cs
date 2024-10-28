using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    public Image tutorial;

    private void Start()
    {
        tutorial = GetComponent<Image>();
    }

    public void OpenTutorial()
    { 
        tutorial.enabled = true;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }
}
