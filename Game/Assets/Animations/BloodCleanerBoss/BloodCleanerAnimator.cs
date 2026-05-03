using UnityEngine;

[RequireComponent(typeof(BloodCleaner))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]

public class BloodCleanerAnimator : MonoBehaviour
{
    private BloodCleaner _bc;
    private Animator _am;
    private SpriteRenderer _sr;

    private void Start()
    {
        _bc = GetComponent<BloodCleaner>();
        _am = GetComponent<Animator>();
        _sr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        
        if (_bc.IsWalking)
        {
            _am.SetBool("IsWalking", true);
        }

        if (_bc.DeltaMove.x > 0)
            _sr.flipX = true;
        else if (_bc.DeltaMove.x < 0)
            _sr.flipX = false;
    }

}
