using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUIController : MonoBehaviour
{
    public GameObject tutorial;
    public GameObject menu;

    public CheckersBaordScript board;

    public void OpenTutorial()
    {
        Debug.Log("Tutorial");
        tutorial.SetActive(true);
    }

    public void CloseTutorial()
    {
        tutorial.SetActive(false);
    }

    public void OpenMenu()
    { 
        menu.SetActive(true);
    }

    public void CloseMenu()
    {
        menu.SetActive(false);
    }

    public void ToggleForcedMove()
    {
        board.forcedMoveActive = !board.forcedMoveActive;
    }
}
