using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public delegate void ChangeSettings();
    public event ChangeSettings OnChangeSettings;

    [SerializeField] private TMP_Dropdown _displayMode;
    [SerializeField] private TMP_Dropdown _resolution;
    [SerializeField] private Toggle _hints;
    [SerializeField] private Button _saveButton;
    [SerializeField] private Slider _volume;

    private void Awake()
    {
        _saveButton.onClick.AddListener(SaveSettings);
        _displayMode.value = PlayerPrefs.GetInt("DisplayMode");
        _resolution.value = PlayerPrefs.GetInt("Resolution");
        _hints.isOn = PlayerPrefs.GetInt("Hints") == 1;
        _volume.value = PlayerPrefs.GetFloat("Volume");
    }

    private void Update()
    {
        if (_displayMode.value == 0)
        {
            _resolution.enabled = false;
            _resolution.captionText.color = Color.HSVToRGB(0, 0, 0.23f);
        }
        else
        {
            _resolution.enabled = true;
            _resolution.captionText.color = Color.HSVToRGB(0, 0, 100);
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetInt("DisplayMode", _displayMode.value);
        PlayerPrefs.SetInt("Resolution", _resolution.value);
        PlayerPrefs.SetInt("Hints", _hints.isOn?1:0);
        PlayerPrefs.SetFloat("Volume", _volume.value);
        PlayerPrefs.Save();
        OnChangeSettings?.Invoke();
    }
}
