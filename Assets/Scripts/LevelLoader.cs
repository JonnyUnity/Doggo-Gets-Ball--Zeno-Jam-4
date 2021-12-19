using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator anim;

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevelAnimation(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadLevel(int levelIndex)
    {
        StartCoroutine(LoadLevelAnimation(levelIndex));
    }

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadLevelAnimation(sceneName));
    }

    public void RestartLevel()
    {
        StartCoroutine(LoadLevelAnimation(SceneManager.GetActiveScene().name));
    }

    public void FadeToBlack()
    {
        StartCoroutine(FadeToBlackAnimation());
    }

    IEnumerator FadeToBlackAnimation()
    {
        anim.SetTrigger("Start");
        yield return new WaitForSeconds(1);
    }



    IEnumerator LoadLevelAnimation(int levelIndex)
    {
        yield return StartCoroutine(FadeToBlackAnimation());
        SceneManager.LoadScene(levelIndex);
    }

    IEnumerator LoadLevelAnimation(string sceneName)
    {
        yield return StartCoroutine(FadeToBlackAnimation());
        SceneManager.LoadScene(sceneName);
    }
}
