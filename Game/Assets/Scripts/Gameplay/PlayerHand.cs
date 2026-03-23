using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class PlayerHand : MonoBehaviour
{
    private Item _activeItem;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            GetComponent<Inventory>().DropActiveItem();
            _activeItem.GetComponent<Interactive>().isInteractive = true;
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
        if (item is FlashlightItem flashlight)
        {
            Inventory.Instance.InventoryWin.FlashLightSliderAppear(flashlight);
        }
    }
    public void HideItem()
    {
        _activeItem.gameObject.SetActive(false);
        if (_activeItem is FlashlightItem flashlight)
        {
            Inventory.Instance.InventoryWin.FlashLightSliderDisappear();
        }
        _activeItem = null;
    }

}

