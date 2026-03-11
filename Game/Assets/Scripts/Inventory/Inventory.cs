using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Interactive;

public class Inventory : MonoBehaviour
{
    private readonly uint _maxSize;
    private uint _size = 0;
    public delegate void AddItemDelegate();
    private event AddItemDelegate OnAddition;
    [SerializeField] private List<Item> _inventoryItems;
    [SerializeField] private List<Item> _startItems;

    private void Awake()
    {
        foreach (var item in _startItems)
        {
            AddItem(item);
        }
    }

    public void PickUp(Item item)
    {
        AddItem(item);
        OnAddition?.Invoke();
    }

    private void AddItem(Item item)
    {
        _inventoryItems.Add(item);
        _size++;
    }
    public uint GetCountOfItems()
    {
        return _size;
    }
    public Item GetItem(int index)
    {
        return _inventoryItems[index];
    }

    public void SetListener(AddItemDelegate listener)
    {
        OnAddition += listener;
    }

    public void RemoveListener()
    {
        OnAddition = null;
    }

}
