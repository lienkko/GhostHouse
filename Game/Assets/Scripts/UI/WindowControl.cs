using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowControl : MonoBehaviour
{
    private int[,] _resolutions = new int[2, 2] { { 800, 600 }, { 1280, 960 } };
    private int _minWidth = 0;
    private int _minHeight = 0;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        bool fullscreen = PlayerPrefs.GetInt("DisplayMode") == 0;
        if (!fullscreen)
        {
            int resolutionIndex = PlayerPrefs.GetInt("Resolution");
            _minWidth = _resolutions[resolutionIndex, 0];
            _minHeight = _resolutions[resolutionIndex, 1];
        }
        Screen.SetResolution(_minWidth, _minHeight, fullscreen);
    }

    private void Update()
    {
        if (Screen.width < _minWidth)
        {
            Screen.SetResolution(_minWidth,Screen.height, false);
        }
        if (Screen.height < _minHeight)
        {
            Screen.SetResolution(Screen.width,_minHeight, false);
        }
        if (Screen.height > Screen.width)
            Screen.SetResolution(_minWidth, _minHeight, false);
    }
}
