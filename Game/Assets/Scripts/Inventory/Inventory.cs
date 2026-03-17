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

    private bool AddItem(Item item)
    {
        if ((_size == MaxSize) && (_activeSlot == 0))
            return false;
        else if (_activeSlot != 0)
        {
            int curSlot = _activeSlot;
            if (_inventoryItems[_activeSlot - 1] != _emptyItem)
                DropActiveItem();
            _inventoryItems[curSlot - 1] = item;
            ChangeActiveSlot(curSlot);
        }
        else
        {
            for (int it = 0; it < MaxSize; it++)
            {
                if (_inventoryItems[it] == _emptyItem)
                {
                    _inventoryItems[it] = item;
                    break;
                }
            }
        }
        _size++;
        return true;
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
    public bool PickUp(Item item)
    {
        bool wasAdded = AddItem(item);
        OnAddition?.Invoke();
        return wasAdded;
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
