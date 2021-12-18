using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuHandler : MonoBehaviour
{

    public GameObject PauseMenu;

    public void ResumeGame()
    {
        GameManager.Instance.ResumeGame();
    }


    public void QuitToMenu()
    {
        GameManager.Instance.QuitToMenu();
    }

}
