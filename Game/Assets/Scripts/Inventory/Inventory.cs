using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private readonly uint _maxSize;
    private uint _size = 0;
    [SerializeField] private List<InventoryItem> _inventoryItems;
    [SerializeField] private List<InventoryItem> _startItems;

    private void Awake()
    {
        foreach (var item in _startItems)
        {
            AddItem(item);
        }
    }

    private void AddItem(InventoryItem item)
    {
        _inventoryItems.Add(item);
        _size++;
    }
    public uint GetCountOfItems()
    {
        return _size;
    }
    public InventoryItem GetItem(int index)
    {
        return _inventoryItems[index];
    }

}
