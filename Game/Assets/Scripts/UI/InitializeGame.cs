using UnityEngine;
using UnityEngine.SceneManagement;

public class InitializeGame : MonoBehaviour
{
    private void Awake()
    {

        if (!PlayerPrefs.HasKey("Launched"))
        {
            PlayerPrefs.SetInt("DisplayMode", 1);
            PlayerPrefs.SetInt("Resolution", 0);
            PlayerPrefs.SetInt("Hints", 1);
            PlayerPrefs.SetFloat("Volume", 1);
            PlayerPrefs.SetInt("Launched", 1);
            PlayerPrefs.Save();
        }
    }

    private void Start()
    {
        SceneManager.LoadScene("main menu");
    }
}
