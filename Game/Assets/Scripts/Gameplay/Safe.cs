using UnityEngine;

public class Safe : MonoBehaviour
{
    [SerializeField] private GameObject OpenText;
    private bool _mayOpen = false;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        OpenText.SetActive(true);
        _mayOpen = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OpenText?.SetActive(false);
        _mayOpen = false;
    }




}
