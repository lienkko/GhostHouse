using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds1_5 = new(1.5f);
    private static readonly int KilledByHandHash = Animator.StringToHash("KilledByHand");
    private static readonly int TrappedHash = Animator.StringToHash("Trapped");
    private PlayerController _playerController;
    private Slider _trapSlider;
    public static bool IsPlayerTrapped { get; private set; } = false;
    private readonly float _trapSpeed = 0.1f;
    private float _trapLeftTime;

    private void Start()
    {
        _trapLeftTime = 0.4f;
        _trapSlider = GameManager.Instance.GameUIFields.TrapSlider.GetComponent<Slider>();
        IsPlayerTrapped = false;
    }
    private void Update()
    {
        if (IsPlayerTrapped)
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
                _playerController.GetComponent<Animator>().SetBool(KilledByHandHash, true);
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
        IsPlayerTrapped = true;
        var am = _playerController.GetComponent<Animator>();
        am.SetBool(TrappedHash, true);
    }
    private void ReleasePlayer()
    {
        IsPlayerTrapped = false;
        _playerController.GetComponent<Animator>().SetBool(TrappedHash, false);
        ShowSlider(false);
        StartCoroutine(UnblockPlayer());
        Inventory.Instance.ChangeAnimation();
    }
    private IEnumerator UnblockPlayer()
    {
        yield return _waitForSeconds1_5;
        GameManager.Instance.BlockPlayer(false);
        Destroy(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerCntrl = collision.GetComponent<PlayerController>();
        if (playerCntrl && !IsPlayerTrapped)
        {
            _playerController = playerCntrl;
            CatchPlayer();
            ShowSlider(true);
            TrapPlayer();
        }
    }
}
