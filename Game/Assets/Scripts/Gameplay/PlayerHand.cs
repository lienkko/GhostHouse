using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class PlayerHand : MonoBehaviour
{

    private Item _activeItem;

    private void Update()
    {
        if (_activeItem)
        {
            _activeItem.transform.position = PlayerController.Instance.transform.position + new Vector3(0, 0.5f, 0);
        }
        if (!GameManager.Instance.CanUseKeyboard)
            return;
        if (Input.GetKeyDown(KeyCode.G))
        {
            DropItem();
        }
        if (Input.GetKeyDown(KeyCode.Space) && _activeItem != null)
        {
            bool needToDestroy = _activeItem.UseAndDestroy();
            if (needToDestroy)
            {
                Item itemToDestroy = _activeItem;
                GetComponent<Inventory>().DropActiveItem();
                Destroy(itemToDestroy.gameObject);
            }
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
    private void DropItem()
    {
        HideItem();
        GetComponent<Inventory>().DropActiveItem();
    }
    public void HideItem()
    {

        if (_activeItem is FlashlightItem)
        {
            Inventory.Instance.InventoryWin.FlashLightSliderDisappear();
        }
        _activeItem = null;
    }

}

