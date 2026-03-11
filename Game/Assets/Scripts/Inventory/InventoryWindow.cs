using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private RectTransform _itemsPanel;

    private void Start()
    {
        Redraw();
    }

    private void Redraw()
    {
        for (var i = 0; i < _inventory.GetCountOfItems(); i++)
        {
            var item = _inventory.GetItem(i);

            var icon = new GameObject(item.Name);
            icon.AddComponent<Image>().sprite = item.Icon;
            icon.transform.SetParent(_itemsPanel.transform, false);
        }
    }
}
