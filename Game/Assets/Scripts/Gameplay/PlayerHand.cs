using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class PlayerHand : MonoBehaviour
{
    public static PlayerHand Instance { get; private set; }
    public Item ActivveItem{get; private set;}

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (ActivveItem)
        {
            ActivveItem.transform.position = PlayerController.Instance.transform.position + new Vector3(0, 0.5f, 0);
        }
        if (!GameManager.Instance.CanUseKeyboard)
            return;
        if (Input.GetKeyDown(KeyCode.G))
        {
            DropItem();
        }
        if (Input.GetKeyDown(KeyCode.Space) && ActivveItem != null)
        {
            bool needToDestroy = ActivveItem.UseAndDestroy();
            if (needToDestroy)
            {
                Item itemToDestroy = ActivveItem;
                GetComponent<Inventory>().DropActiveItem();
                Destroy(itemToDestroy.gameObject);
            }
        }
        
    }
    public void TakeItem(Item item)
    {
        ActivveItem = item;
        ActivveItem.GetComponent<Interactive>().isInteractive = false;
        ActivveItem.gameObject.SetActive(true);
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

        if (ActivveItem is FlashlightItem)
        {
            Inventory.Instance.InventoryWin.FlashLightSliderDisappear();
        }
        ActivveItem = null;
    }

}

