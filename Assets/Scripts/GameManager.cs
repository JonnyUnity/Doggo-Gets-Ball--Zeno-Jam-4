using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.sceneLoaded += LoadLevelRequirements;

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

    private void LoadLevelRequirements(Scene scene, LoadSceneMode mode)
    {
        if (LevelRequirements.TryGetValue(scene.name, out LevelRequirements levelReqs))
        {
            TilesRemaining = levelReqs.TilesRemaining;
            Debug.Log("Loaded Tiles remaining count: " + TilesRemaining);
        }

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
