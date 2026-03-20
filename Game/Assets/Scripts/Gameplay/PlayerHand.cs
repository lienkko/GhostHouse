using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private Item _activeItem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetComponent<Inventory>().DropActiveItem();
            HideItem();
        }
        if (Input.GetKeyDown(KeyCode.Space) && _activeItem != null)
        {
            bool needToDestroy = _activeItem.UseAndDestroy();
            if (needToDestroy)
            {
                Item itemToDestroy = _activeItem;
                GetComponent<Inventory>().DropActiveItem();
                Destroy(_activeItem.gameObject);
            }
        }
        if (_activeItem)
        {
            _activeItem.transform.position = PlayerController.Instance.transform.position + new Vector3(0, 0.5f, 0);
        }
    }
    public void TakeItem(Item item)
    {
        _activeItem = item;
        _activeItem.GetComponent<Interactive>().isInteractive = false;
        _activeItem.gameObject.SetActive(true);
    }
    public void HideItem()
    {
        _activeItem.gameObject.SetActive(false);
        _activeItem.GetComponent<Interactive>().isInteractive = true;

        _activeItem = null;
    }

}

