using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private Item _activeItem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _activeItem != null)
        {
            bool needToDestroy = _activeItem.UseAndDestroy();
            if (needToDestroy)
            {
                var itemToDestroy = _activeItem;
                GetComponent<Inventory>().DropActiveItem();
                Destroy(_activeItem.gameObject);
            }
        }
    }
    public void TakeItem(Item item)
    {
        _activeItem = item;
    }
    public void HideItem()
    {
        _activeItem = null;
    }

}

