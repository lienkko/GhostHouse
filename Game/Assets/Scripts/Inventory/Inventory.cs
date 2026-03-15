using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Item _emptyItem;
    private List<Item> _inventoryItems;
    private uint _size = 0;
    private int _activeSlot = 0;
    public readonly uint MaxSize = 4;
    public delegate void AddItemDelegate();
    private event AddItemDelegate OnAddition;

    private void Awake()
    {
        _emptyItem = new GameObject("emptyItem").AddComponent<Item>();
        _emptyItem.gameObject.SetActive(false);
        _inventoryItems = new List<Item>();
        for (int i = 0; i < MaxSize; i++)
            _inventoryItems.Add(_emptyItem);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
            DropActiveItem();
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
            GetComponent<PlayerHand>().HideItem();
            return;
        }
        if (_inventoryItems[index - 1] == _emptyItem)
            return;
        _activeSlot = index;
        GetComponent<PlayerHand>().TakeItem(_inventoryItems[index - 1]);
    }

    private void AddItem(Item item)
    {
        if (_activeSlot != 0)
        {
            _inventoryItems[_activeSlot - 1] = item;
            _size++;
        }
        for (int it = 0; it < MaxSize - 1; it++)
        {
            if (_inventoryItems[it] == _emptyItem)
            {
                _inventoryItems[it] = item;
                _size++;
                return;
            }
        }
        _inventoryItems[3] = item;
        _size++;
    }
    private void DeleteItem(int index)
    {
        _inventoryItems[index] = _emptyItem;
        _size--;
    }
    public void DropActiveItem()
    {
        if (_activeSlot == 0)
            return;
        _inventoryItems[_activeSlot - 1].transform.position = gameObject.transform.position;
        _inventoryItems[_activeSlot - 1].gameObject.SetActive(true);
        DeleteItem(_activeSlot - 1);
        _activeSlot = 0;

    }
    public Item GetEmptyItem()
    {
        return _emptyItem;
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
