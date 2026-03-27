using UnityEngine;

[RequireComponent(typeof(Spider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class SpiderAnimator : MonoBehaviour
{
    private Spider _sp;
    private Animator _am;
    private SpriteRenderer _sr;


    private void Start()
    {
        _sp = GetComponent<Spider>();
        _am = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_sp.Agent.speed != 0 && _sp.IsWalking){
            _am.SetBool("IsWalking", true);
        }
        else{
            _am.SetBool("IsWalking", false);
        }
        if (_sp.Agent.speed != 0 && _sp.IsRunning)
        {
            _am.SetBool("IsRunning", true);
        }
        else
        {
            _am.SetBool("IsRunning",false);
        }
        if (_sp.DeltaMove == Vector3.zero)
        {
            _am.SetBool("IsWalking", false);
            _am.SetBool("IsRunning",false);
        }

        if (_sp.DeltaMove.x > 0)
            _sr.flipX = true;
        else if (_sp.DeltaMove.x < 0)
            _sr.flipX = false;

    }

}
