using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
//using UnityEngine.UI;

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

    // Audio
    private AudioSource AudioSource;

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
        SceneManager.LoadScene(1);
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
        SceneManager.LoadScene(0);
    }

    //private void LoadTilesRemaining(Scene scene, LoadSceneMode mode)
    //{
    //    if (LevelTiles.TryGetValue(scene.name, out int tilesRemaining))
    //    {
    //        TilesRemaining = LevelTiles[scene.name];
    //        Debug.Log("Loaded Tiles remaining count: " + TilesRemaining);
    //    }        
    //}

    private void LoadLevel(Scene scene, LoadSceneMode mode)
    {
        Camera cam = Camera.main;

        if (LevelRequirements.TryGetValue(scene.name, out CurrentLevel))
        {
            TilesRemaining = CurrentLevel.TilesRemaining;
            Debug.Log("Loaded Tiles remaining count: " + TilesRemaining);

            // Update Camera
            cam.orthographicSize = CurrentLevel.CameraSize;

            var grid = GameObject.Find("Grid");

            //var wallsTileMap = grid.transform.Find("Walls").gameObject;
            var wallsTileMap = GameObject.Find("Walls").gameObject;

            float cameraX, cameraY;
            var localBounds = wallsTileMap.GetComponent<TilemapRenderer>().bounds;
            cameraX = localBounds.min.x + (localBounds.max.x - localBounds.min.x) / 2;
            cameraY = localBounds.min.y + (localBounds.max.y - localBounds.min.y) / 2;

            cam.transform.position = new Vector3(cameraX, cameraY, -1f);



            //foreach (var tm in tilemaps)
            //{
            //    if (tm.CompareTag("Wall"))
            //    {
            //        var localBounds = tm.GetComponent<TilemapRenderer>().bounds;
            //        cameraX = localBounds.min.x + (localBounds.max.x - localBounds.min.x) / 2;
            //        cameraY = localBounds.min.y + (localBounds.max.y - localBounds.min.y) / 2;

            //        cam.transform.position = new Vector3(cameraX, cameraY, -1f);
            //    }
            //}

            GetFirstTryFlag();
            //FirstTry = 1;
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
            Debug.Log(" Player = " + enabled);
            //playerVar.ToggleActive(enabled);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    // Level is only complete if the star is collected as the last square.
    public bool IsLevelComplete()
    {
        //return (TilesRemaining == 0);
        return (TilesRemaining == 1);   // last tile to cover!
    }


    public void LoadNextLevel()
    {
        FirstTry = 1;
        SetFirstTryFlag();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
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
        // opening scene transisiton?
        // display start level messages...
        Debug.Log("Level Starting");


        yield return StartCoroutine(ShowMessages(CurrentLevel.StartLevelMessages, CurrentLevel.StartLevelWait));

        
        //yield return new WaitForSeconds(1);
    }

    private IEnumerator LevelPlaying()
    {
        Debug.Log("Level Playing");
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
        Debug.Log("Level Ending");
        EnableDisablePlayer(false);

        yield return StartCoroutine(ShowMessages(CurrentLevel.EndLevelMessages, CurrentLevel.EndLevelWait));

        //yield return new WaitForSeconds(1);
        LoadNextLevel();
    }

    private IEnumerator LevelRestarting()
    {
        // any leaving messages, scene transistion
        Debug.Log("Level Restarting");
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
        //var textBackground = background.transform.GetChild(0);
        //var messageTextGO = textBackground.GetChild(0);
        //var messageText = messageTextGO.GetComponent<TextMeshProUGUI>();
        //var messageText = background.GetComponentInChildren<TMP_Text>();
        //var messageText = GetMessageText();
        var messageText = GameObject.FindGameObjectsWithTag("MessageText")[0].GetComponent<TextMeshProUGUI>();

        //GetMessageText();
        //var textBackground = MessageCanvas.transform.Find("TextBackground").gameObject;
        //textBackground.SetActive(true);

        foreach (var msg in messages)
        {
            messageText.text = msg;
            yield return new WaitForSeconds(messagePause);
        }
        messageText.text = "";
        background.SetActive(false);
        //textBackground.SetActive(false);
    }

    private void GetMainCanvas()
    {
        var canvas = GameObject.FindGameObjectWithTag("MainCanvas");
        if (canvas != null)
        {
            MessageCanvas = canvas.transform.Find("Messages").gameObject;
        }
    }

    private void GetMessageText()
    {
        var obj = GameObject.FindGameObjectsWithTag("MessageText").GetValue(0);
    }

    //private GameObject GetMainCanvas()
    //{
    //    var canvas = GameObject.FindGameObjectWithTag("MainCanvas");
    //    if (canvas != null)
    //    {
    //        var trans = canvas.transform.Find("Messages");
    //        if (trans != null)
    //        {
    //            return trans.gameObject;
    //        }
    //    }

    //    return null;
    //}

    public void SetFailState(string message)
    {
        FirstTry = 0;
        SetFirstTryFlag();
        FailMessage = message;
        State = GameState.Dead;
    }

    public void HitWall()
    {
        // randomise message?
        SetFailState(CurrentLevel.FailMessage_Dead);
    }

    public void HitOwnBody()
    {
        // randomise message?
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

public enum PlayerState
{
    Alive,
    HitWall,
    HitOwnBody
}
