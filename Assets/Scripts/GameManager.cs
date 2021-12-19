using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;

    public Dictionary<string, LevelRequirements> LevelRequirements;
    public LevelRequirements CurrentLevel;

    [HideInInspector]
    public int TilesRemaining;
    private string FailMessage;
    private int FirstTry = 1;

    private GameObject PauseMenu;
    private GameObject MessageCanvas;

    private AudioSource AudioSource;
    private LevelLoader LevelLoader;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        State = GameState.MainMenu;
        Instance = this;
        AudioSource = GetComponent<AudioSource>();
        InitLevelRequirements();

        SceneManager.sceneLoaded += LoadLevel;

        DontDestroyOnLoad(gameObject);
    }

    private void GetLevelLoader()
    {
        if (LevelLoader == null)
        {
            LevelLoader = GameObject.FindObjectOfType<LevelLoader>();
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (State == GameState.InPlay)
            {
                PauseGame();
            }
            else if (State == GameState.Paused)
            {
                ResumeGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && State == GameState.InPlay)
        {
            FirstTry = 0;
            SetFirstTryFlag();
            GameManager.Instance.RestartLevel();
        }
    }

    #region Pause Menu

    public void PauseGame()
    {
        State = GameState.Paused;
        if (PauseMenu == null)
        {
            PauseMenu = GetPauseMenu();
        }
        PauseMenu.SetActive(true);
    }

    public void ResumeGame()
    {
        if (PauseMenu == null)
        {
            PauseMenu = GetPauseMenu();
        }
        PauseMenu.SetActive(false);

        State = GameState.InPlay;
    }

    private GameObject GetPauseMenu()
    {
        GameObject pauseMenu = null;

        var canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        if (canvas != null)
        {
            var trans = canvas.transform.Find("PauseMenu");
            if (trans != null)
            {
                return trans.gameObject;
            }
        }

        return pauseMenu;
    }

#endregion

    private void InitLevelRequirements()
    {
        LevelRequirements = new Dictionary<string, LevelRequirements>();
        Object[] levelReqs = Resources.LoadAll("Requirements", typeof(LevelRequirements));
        Debug.Log("Found " + levelReqs.Length + " level requirements!");

        foreach (LevelRequirements o in levelReqs)
        {
            LevelRequirements[o.name] = o;
        }
    }


    public void StartGame()
    {
        GetLevelLoader();
        LevelLoader.LoadLevel(1);
    }


    public void QuitGame()
    {
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#else
//    Application.Quit();
//#endif
        Application.Quit();
    }


    public void QuitToMenu()
    {
        GetLevelLoader();
        LevelLoader.LoadLevel(0);
    }


    private void LoadLevel(Scene scene, LoadSceneMode mode)
    {
        Camera cam = Camera.main;

        if (LevelRequirements.TryGetValue(scene.name, out CurrentLevel))
        {
            TilesRemaining = CurrentLevel.TilesRemaining;
            Debug.Log("Loaded Tiles remaining count: " + TilesRemaining);

            // Update Camera
            cam.orthographicSize = CurrentLevel.CameraSize;

            var wallsTileMap = GameObject.Find("Walls").gameObject;
            var localBounds = wallsTileMap.GetComponent<TilemapCollider2D>().bounds;
            cam.transform.position = new Vector3(localBounds.center.x, localBounds.center.y, -1);

            GetFirstTryFlag();
            EnableDisablePlayer(false);

            if (!AudioSource.isPlaying)
            {
                AudioSource.Play();
            }

            StartCoroutine(GameLoop());

        }
        else
        {
            if (AudioSource.isPlaying)
            {
                AudioSource.Stop();
            }
        }
        

    }

    private void EnableDisablePlayer(bool enabled)
    {
        DogController playerVar = null;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerVar = player.GetComponent<DogController>();
        }

        if (playerVar != null)
        {
            playerVar.enabled = enabled;
        }

                
    }

    private void GetFirstTryFlag()
    {
        FirstTry = PlayerPrefs.GetInt("FirstTry", 1);
        SetFirstTryFlag();
    }

    private void SetFirstTryFlag()
    {
        PlayerPrefs.SetInt("FirstTry", FirstTry);
    }

    public void RestartLevel()
    {
        GetLevelLoader();
        LevelLoader.RestartLevel();
    }


    // Level is only complete if the ball is collected as the last square.
    public bool IsLevelComplete()
    {
        return (TilesRemaining == 1);   // last tile to cover!
    }


    public void LoadNextLevel()
    {
        FirstTry = 1;
        SetFirstTryFlag();

        GetLevelLoader();
        LevelLoader.LoadNextLevel();
    }
        

    private IEnumerator GameLoop()
    {

        if (FirstTry == 1)
        {
            yield return StartCoroutine(LevelStarting());
        }

        yield return StartCoroutine(LevelPlaying());

        if (State == GameState.LevelEnding)
        {
            yield return StartCoroutine(LevelEnding());
        }
        else
        {
            yield return StartCoroutine(LevelRestarting());
        }

        

    }


    private IEnumerator LevelStarting()
    {
        yield return StartCoroutine(ShowMessages(CurrentLevel.StartLevelMessages, CurrentLevel.StartLevelWait));
    }

    private IEnumerator LevelPlaying()
    {
        EnableDisablePlayer(true);
        State = GameState.InPlay;

        // completing the level or dying will end this loop
        while (State == GameState.InPlay || State == GameState.Paused)
        {
            yield return null;
        }
    }

    private IEnumerator LevelEnding()
    {
        // any leaving messages, scene transistion
        EnableDisablePlayer(false);

        yield return StartCoroutine(ShowMessages(CurrentLevel.EndLevelMessages, CurrentLevel.EndLevelWait));

        LoadNextLevel();
    }

    private IEnumerator LevelRestarting()
    {
        // any leaving messages, scene transistion
        EnableDisablePlayer(false);
        
        yield return StartCoroutine(ShowMessages(new string[] { FailMessage }, CurrentLevel.FailWait));

        RestartLevel();
    }

    private IEnumerator ShowMessages(string[] messages, int messagePause)
    {
        if (MessageCanvas == null)
        {
            GetMainCanvas();
        }

        MessageCanvas.SetActive(true);  // in case it's inactive
        
        var background = MessageCanvas.transform.Find("Background").gameObject;
        background.SetActive(true);
        
        var messageText = GameObject.FindGameObjectsWithTag("MessageText")[0].GetComponent<TextMeshProUGUI>();
        foreach (var msg in messages)
        {
            messageText.text = msg;
            yield return new WaitForSeconds(messagePause);
        }
        messageText.text = "";
        background.SetActive(false);
    }


    private void GetMainCanvas()
    {
        var canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        if (canvas != null)
        {
            MessageCanvas = canvas.transform.Find("Messages").gameObject;
        }
    }


    public void SetFailState(string message)
    {
        FirstTry = 0;
        SetFirstTryFlag();
        FailMessage = message;
        State = GameState.Dead;
    }


    public void HitWall()
    {
        SetFailState(CurrentLevel.FailMessage_Dead);
    }


    public void HitOwnBody()
    {
        SetFailState(CurrentLevel.FailMessage_Dead);
    }


    public void CollectedBallEarly()
    {
        SetFailState(CurrentLevel.FailMessage_TooSoon);
    }


    public void CompleteLevel()
    {
        State = GameState.LevelEnding;
    }


}

public enum GameState
{
    MainMenu,
    LevelStarting,
    InPlay,
    Paused,
    Dead,
    LevelEnding
}