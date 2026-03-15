using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Image[] _inventoryIcons;
    [SerializeField] private Sprite[] _inventoryBG;

    private void Start()
    {
        _inventory.SetListener(Redraw);
        Redraw();
    }

    private void Redraw()
    {
        GetComponent<Image>().sprite = _inventoryBG[_inventory.GetActiveSlot()];
        for (var i = 0; i < _inventory.MaxSize; i++)
        {
            var item = _inventory.GetItem(i);
            if (item == null)
                _inventoryIcons[i].gameObject.SetActive(false);
            else
            {
                _inventoryIcons[i].sprite = item.GetIcon();
                _inventoryIcons[i].gameObject.SetActive(true);
            }
        }
    }
}
