using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<Item> _inventoryItems;
    [SerializeField] private List<Item> _startItems;
    private uint _size = 0;
    private int _activeSlot = 0;
    public readonly uint MaxSize = 4;
    public delegate void AddItemDelegate();
    private event AddItemDelegate OnAddition;
    private void Awake()
    {
        foreach (var item in _startItems)
        {
            AddItem(item);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ChangeActiveSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            ChangeActiveSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            ChangeActiveSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            ChangeActiveSlot(4);
        OnAddition?.Invoke();
        return;
    }

    private void ChangeActiveSlot(int index)
    {
        if (_activeSlot == index)
        {
            _activeSlot = 0;
            return;
        }
        _activeSlot = index;
    }

    private void AddItem(Item item)
    {
        _inventoryItems.Add(item);
        _size++;
    }

    public int GetActiveSlot()
    {
        return _activeSlot;
    }
    public uint GetCountOfItems()
    {
        return _size;
    }
    public void PickUp(Item item)
    {
        AddItem(item);
        OnAddition?.Invoke();
    }

    public Item GetItem(int index)
    {
        return index < _size ? _inventoryItems[index] : null;
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
