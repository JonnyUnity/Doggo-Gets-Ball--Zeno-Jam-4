using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIHandler : MonoBehaviour
{

    public void StartGame()
    {
        GameManager.Instance.StartGame();
    }

    public void Exit()
    {
        GameManager.Instance.QuitGame();
    }

    public void BackToStart()
    {
        GameManager.Instance.QuitToMenu();
    }




}
