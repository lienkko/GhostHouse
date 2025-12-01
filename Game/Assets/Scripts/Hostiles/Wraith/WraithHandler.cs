using System.Collections;
using UnityEngine;

public class WraithHandler : MonoBehaviour
{
    public static WraithHandler Instance { get; private set; }

    [SerializeField] private GameObject _wraithModel;

    [SerializeField] private AudioClip _whispers;
    [SerializeField] private AudioClip _iSeeYouWhisper;
    private AudioSource _audioSource;

    private float _distance = 0;
    private float _remainingDsitance = 0;

    private Vector3 _startPoint = Vector3.zero;
    private Vector3 _endPoint = Vector3.zero;

    public bool IsMoving { get; private set; } = false;

    private bool _isWhispering = false;

    private void Awake()
    {
        Instance = this;

        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = 0.3f;
    }

    private void FixedUpdate()
    {
        if (IsMoving)
        {
            _wraithModel.transform.position = Vector3.Lerp(_startPoint, _endPoint, 1-(_remainingDsitance/_distance));
            _remainingDsitance -= 7f * Time.deltaTime;
            if (_remainingDsitance < 0.8 * _distance && !_isWhispering) 
            { 
                GameManager.Instance.TurnOffLights(true); 
            }
            if (_remainingDsitance < 0.2 * _distance && !_isWhispering)
            {
                GameManager.Instance.GMAudioSource.PlayOneShot(_iSeeYouWhisper);
                _isWhispering = true;
            }
        }
        
        if (IsMoving && (_wraithModel.transform.position == _endPoint))
        {
            IsMoving = false;
            _isWhispering = false;
            OpenDoors();
            _wraithModel.SetActive(false);
        }
    }

    private void OpenDoors()
    {
        RoomData currentRoomData = RoomsManager.Instance.CurrentRoom.GetComponent<RoomData>();
        if (!currentRoomData.PreviousRoomDoor.isDoorLocked)
            currentRoomData.PreviousRoomDoor.GetComponent<Interactive>().isInteractive = true;
        if (!currentRoomData.NextRoomDoor.isDoorLocked)
            currentRoomData.NextRoomDoor.GetComponent<Interactive>().isInteractive = true;
    }
    private void CloseDoors()
    {
        RoomData currentRoomData = RoomsManager.Instance.CurrentRoom.GetComponent<RoomData>();
        currentRoomData.PreviousRoomDoor.GetComponent<Interactive>().isInteractive = false;
        currentRoomData.NextRoomDoor.GetComponent<Interactive>().isInteractive = false;
    }

    public void StartWraith(Vector3 point1, Vector3 point2)
    {
        CloseDoors();

        _startPoint = point1;
        _endPoint = point2;

        StartCoroutine(WraithWaiting());
    }

    public IEnumerator WraithWaiting()
    {
        yield return new WaitForSeconds(6);

        _wraithModel.transform.position = _startPoint;
        _wraithModel.SetActive(true);

        _audioSource.PlayOneShot(_whispers);

        //Start flight
        _distance = _remainingDsitance = Vector3.Distance(_startPoint, _endPoint);
        IsMoving = true;
    }
}
