using System.Collections;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverText;
    [SerializeField] private Text _healthPointsField;
    [SerializeField] private Ghost _ghost;
    [SerializeField] private AudioClip _blinkLightsSound;

    
    private AudioSource _audioSource;
    private GameObject _currentRoom;
    private DoorController _startedDoor;
    
    public TextMeshProUGUI OpenDoorText;
    public TextMeshProUGUI OpenSafeText;
    public TextMeshProUGUI HideText;
    public TextMeshProUGUI StartGameText;
    public Image LockedText;

    private void Awake()
    {
        PlayerController.OnDeath += Death;
        PlayerController.OnDamage += ChangeHp;
        _ghost.OnStartGame += StartGame;
        _startedDoor = FindAnyObjectByType<DoorController>();
        _currentRoom = FindAnyObjectByType<RoomData>().gameObject;
        _audioSource = GetComponent<AudioSource>();
    }

    private void StartGame()
    {
        _startedDoor.GetComponent<Interactive>().isInteractive = true;
        StartCoroutine(BlinkLights(false));
    }

    private void Death()
    {
        _gameOverText.SetActive(true);
    }

    private void ChangeHp(int dmg, int hp)
    {
        _healthPointsField.text = hp.ToString();
    }

    private IEnumerator BlinkLights(bool turnOff = false)
    {
        int countOfBlinks = 0;
        Transform lightsTransform = _currentRoom.transform.Find("Lights");
        int numOfChilds = lightsTransform.childCount;
        _audioSource.PlayOneShot(_blinkLightsSound);
        while (countOfBlinks < 50)
        {
            for(int i = 0; i < numOfChilds; i++)
            {
                lightsTransform.GetChild(i).GetComponent<Light2D>().intensity = Random.Range(0.2f,1.2f);
            }
            countOfBlinks++;
            yield return new WaitForSeconds(Time.deltaTime*6);
        }
        for (int i = 0; i < numOfChilds; i++)
        {
            if (turnOff)
                lightsTransform.GetChild(i).GetComponent<Light2D>().intensity = 0;
            else
                lightsTransform.GetChild(i).GetComponent<Light2D>().intensity = 0.9f;
        }
        _audioSource.Stop();
    }
    
}
