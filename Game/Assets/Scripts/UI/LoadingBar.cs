using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private Slider _loadingBar;
    [SerializeField] private TextMeshProUGUI _loadingText;
    [SerializeField] private string _sceneName;

    private void Start()
    {
        StartCoroutine(LoadScene(_sceneName));
    }

private IEnumerator LoadScene(string scene)
{
    AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
    operation.allowSceneActivation = false;

    while (!operation.isDone)
    {
        float progress = Mathf.Clamp01(operation.progress / 0.9f);
        _loadingBar.value = progress;
            _loadingText.text = $"{(int) progress}%";
        if (operation.progress >= 0.9f)
        {
            operation.allowSceneActivation = true;
        }
        yield return null;
    }
}
}
