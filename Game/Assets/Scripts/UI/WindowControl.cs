using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowControl : MonoBehaviour
{
    private int[,] _resolutions = new int[2, 2] { { 800, 600 }, { 1280, 960 } };
    private int _minWidth = 800;
    private int _minHeight = 600;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
