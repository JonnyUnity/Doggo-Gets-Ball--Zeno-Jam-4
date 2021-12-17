using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;

    public Dictionary<string, int> LevelTiles;
    public Dictionary<string, LevelRequirements> LevelRequirements;
    public int TilesRemaining;



    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        State = GameState.MainMenu;
        Instance = this;
        InitLevelTiles();
        //SceneManager.sceneLoaded += LoadTilesRemaining;
        SceneManager.sceneLoaded += LoadLevel;

        DontDestroyOnLoad(gameObject);
    }

    private void InitLevelTiles()
    {
        LevelTiles = new Dictionary<string, int>();
        // Load required moves here...
        LevelTiles["Level1"] = 2;
        LevelTiles["Level2"] = 8;

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
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit;
#endif
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadTilesRemaining(Scene scene, LoadSceneMode mode)
    {
        if (LevelTiles.TryGetValue(scene.name, out int tilesRemaining))
        {
            TilesRemaining = LevelTiles[scene.name];
            Debug.Log("Loaded Tiles remaining count: " + TilesRemaining);
        }        
    }

    private void LoadLevel(Scene scene, LoadSceneMode mode)
    {
        Camera cam = Camera.main;

        if (LevelRequirements.TryGetValue(scene.name, out LevelRequirements levelReqs))
        {
            TilesRemaining = levelReqs.TilesRemaining;
            Debug.Log("Loaded Tiles remaining count: " + TilesRemaining);

            // Update Camera
            cam.orthographicSize = levelReqs.CameraSize;
        }

        var grid = GameObject.Find("Grid");

        var tilemaps = grid.GetComponentsInChildren<Tilemap>();

        float cameraX, cameraY;
        foreach (var tm in tilemaps)
        {
            if (tm.CompareTag("Wall"))
            {
                var localBounds = tm.GetComponent<TilemapRenderer>().bounds;
                cameraX = localBounds.min.x + (localBounds.max.x - localBounds.min.x) / 2;
                cameraY = localBounds.min.y + (localBounds.max.y - localBounds.min.y) / 2;

                cam.transform.position = new Vector3(cameraX, cameraY, -1f);
            }
        }

        

        // Welcome message?


        State = GameState.InPlay;
    }


    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Level is only complete if the star is collected as the last square.
    public bool IsLevelComplete()
    {
        return (TilesRemaining == 0);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void Update()
    {
        
    }
}

public enum GameState
{
    MainMenu,
    InPlay,
    Paused
}
