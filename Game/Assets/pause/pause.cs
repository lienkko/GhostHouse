using System.Collections;

using System.Collections.Generic;

using UnityEngine;

using UnityEngine.SceneManagement;

public class pause : MonoBehaviour

{

    public bool paused;

    public GameObject pauseMenu;

    void Start()

    {


    }


    

    void Update()

    {

        if (Input.GetKeyDown(KeyCode.Escape))

        {

            if (paused)

            {

                Resume();

            }

            else

            {

                Pause();

            }

        }

    }



    public void Resume()

    {

        paused = false;

        pauseMenu.SetActive(false);

        Time.timeScale = 1f;


    }



    public void Pause()

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