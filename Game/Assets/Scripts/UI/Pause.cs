using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public bool paused;
    public GameObject pauseMenu;

    void Update()
    {
        
    }

    public void Resume()
    {
        paused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void PauseGame()
    {
        paused = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void toMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("main menu");
    }
}
