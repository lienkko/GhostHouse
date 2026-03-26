using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    private PlayerController _playerController;
    [SerializeField] private Slider _trapSlider;
    private bool _isPlayerTrapped = false;
    private readonly float _trapSpeed = 0.1f;
    private float _trapLeftTime = 0.4f;

    private void Update()
    {
        if (_isPlayerTrapped)
        {
            _trapLeftTime -= _trapSpeed * Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space) && !Pause.IsPaused)
            {
                _trapLeftTime += _trapSpeed / 2;
            }
            if (_trapLeftTime >= 1)
            {
                ReleasePlayer();
            }
            if (_trapLeftTime <= 0)
            {
                _playerController.InflictDamage(100);
                ShowSlider(false);
            }
            _trapSlider.value = _trapLeftTime;
        }

    }
    private void CatchPlayer()
    {
        GameManager.Instance.BlockPlayer(true);
        _playerController.gameObject.transform.position = transform.position;
    }
    private void ShowSlider(bool state)
    {
        _trapSlider.gameObject.SetActive(state);
        _trapSlider.value = _trapLeftTime;
    }
    private void TrapPlayer()
    {
        _isPlayerTrapped = true;
        _playerController.GetComponent<Animator>().SetBool("Trapped", true);
    }
    private void ReleasePlayer()
    {
        _isPlayerTrapped = false;
        _playerController.GetComponent<Animator>().SetBool("Trapped", false);
        GameManager.Instance.BlockPlayer(false);
        ShowSlider(false);
        Destroy(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerCntrl = collision.GetComponent<PlayerController>();
        if (playerCntrl && !_isPlayerTrapped)
        {
            _playerController = playerCntrl;
            CatchPlayer();
            ShowSlider(true);
            TrapPlayer();
        }
    }
}
