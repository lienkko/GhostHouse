using System.ComponentModel.Design;
using UnityEngine;

[RequireComponent(typeof(Inventory))]
public class PlayerHand : MonoBehaviour
{
    public static PlayerHand Instance { get; private set; }
    public Item ActiveItem { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    private void Update()
    {
        if (ActiveItem)
        {
            ActiveItem.transform.position = PlayerController.Instance.transform.position + new Vector3(0, 0.5f, 0);
        }
        if (!GameManager.Instance.CanUseKeyboard)
            return;
        if (Input.GetKeyDown(KeyCode.G))
        {
            DropItem();
        }
        if (Input.GetKeyDown(KeyCode.Space) && ActiveItem != null)
        {
            bool needToDestroy = ActiveItem.UseAndDestroy();
            if (needToDestroy)
            {
                Item itemToDestroy = ActiveItem;
                GetComponent<Inventory>().DropActiveItem();
                Destroy(itemToDestroy.gameObject);
            }
        }

    }
    public void TakeItem(Item item)
    {
        ActiveItem = item;
        ActiveItem.GetComponent<Interactive>().isInteractive = false;
        ActiveItem.gameObject.SetActive(true);
        if (item is IChargeableItem chargeableItem)
        {
            Inventory.Instance.InventoryWin.FlashLightSliderAppear(chargeableItem);
        }
        ActiveItem.GetComponent<Interactive>().isInteractive = false;
    }
    private void DropItem()
    {
        HideItem();
        GetComponent<Inventory>().DropActiveItem();
    }
    public void HideItem()
    {

        if (ActiveItem is IChargeableItem)
        {
            Inventory.Instance.InventoryWin.FlashLightSliderDisappear();
        }
        ActiveItem.GetComponent<Interactive>().isInteractive = true;
        ActiveItem = null;
    }

}

