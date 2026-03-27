using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerHand))]
public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    [SerializeField] private InventoryWindow _inventoryWin;
    public InventoryWindow InventoryWin { get { return _inventoryWin; } }
    private Item _emptyItem;
    public List<Item> InventoryItems { get; private set; }
    private uint _size = 0;
    private int _activeSlot = 0;
    public readonly uint MaxSize = 4;
    public delegate void AddItemDelegate();
    private event AddItemDelegate OnAddition;


    private void Awake()
    {
        Instance = this;
        _emptyItem = new GameObject("emptyItem").AddComponent<Item>();
        _emptyItem.gameObject.SetActive(false);
        InventoryItems = new List<Item>();
        for (int i = 0; i < MaxSize; i++)
            InventoryItems.Add(_emptyItem);
    }
    private void Update()
    {
        if (!GameManager.Instance.CanUseKeyboard)
            return;
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
            ShutDownSlot(index);
            return;
        }
        if (InventoryItems[index - 1] == _emptyItem)
            return;
        if (_activeSlot != 0)
            ShutDownSlot(_activeSlot);
        _activeSlot = index;
        GetComponent<PlayerHand>().TakeItem(InventoryItems[index - 1]);
    }
    private void ShutDownSlot(int index)
    {
        InventoryItems[index - 1].Hide();
        _activeSlot = 0;
        GetComponent<PlayerHand>().HideItem();
    }
    private bool AddItem(Item item)
    {
        if ((_size == MaxSize) && (_activeSlot == 0))
            return false;
        else if (_activeSlot != 0)
        {
            int curSlot = _activeSlot;
            if (InventoryItems[_activeSlot - 1] != _emptyItem)
                DropActiveItem();
            InventoryItems[curSlot - 1] = item;
            ChangeActiveSlot(curSlot);
        }
        else
        {
            for (int it = 0; it < MaxSize; it++)
            {
                if (InventoryItems[it] == _emptyItem)
                {
                    InventoryItems[it] = item;
                    break;
                }
            }
        }
        _size++;
        return true;
    }
    private void DeleteItem(int index)
    {
        InventoryItems[index] = _emptyItem;
        _size--;
    }
    public void DropActiveItem()
    {
        if (_activeSlot == 0)
            return;
        InventoryItems[_activeSlot - 1].transform.position = gameObject.transform.position;
        InventoryItems[_activeSlot - 1].GetComponent<Interactive>().isInteractive = true;
        InventoryItems[_activeSlot - 1].Hide();
        InventoryItems[_activeSlot - 1].gameObject.SetActive(true);
        InventoryItems[_activeSlot - 1].GetComponent<Interactive>().isInteractive = true;
        InventoryItems[_activeSlot - 1].transform.SetParent(RoomsManager.Instance.CurrentRoom.transform);
        InventoryItems[_activeSlot - 1].GetComponent<SpriteRenderer>().sortingLayerName = "Default";
        InventoryItems[_activeSlot - 1].GetComponent<SpriteRenderer>().sortingOrder = 0;
 
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
        if (!item.IsCollectable)
        {
            if (item.UseAndDestroy())
            {
                Destroy(item.gameObject);
            }
            return false;
        }
        bool wasAdded = AddItem(item);
        if (wasAdded)
        item.GetComponent<SpriteRenderer>().sortingLayerName = "Layer For Player";
        item.GetComponent<SpriteRenderer>().sortingOrder = 11;
        item.transform.SetParent(transform);
        item.transform.localPosition = new Vector3(0.1f,0.5f,0);
        OnAddition?.Invoke();
        return wasAdded;
    }

    public Item GetItem(int index)
    {
        return InventoryItems[index];
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
