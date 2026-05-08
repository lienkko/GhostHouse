using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }
    [SerializeField] private InventoryWindow _inventoryWin;
    public InventoryWindow InventoryWin { get { return _inventoryWin; } }
    private InventoryItem _emptyItem;
    private InventoryItem _activeItem;
    public List<InventoryItem> InventoryItems { get; private set; }
    private uint _size = 0;
    private int _activeSlot = 0;
    public readonly uint MaxSize = 4;
    public delegate void AddItemDelegate();
    private event AddItemDelegate OnAddition;


    private void Awake()
    {
        Instance = this;
        _emptyItem = new GameObject("emptyItem").AddComponent<Empty>();
        _emptyItem.gameObject.SetActive(false);
        InventoryItems = new List<InventoryItem>();
        for (int i = 0; i < MaxSize; i++)
            InventoryItems.Add(_emptyItem);
    }
    private void Start()
    {
        ChangeAnimation();
    }
    private void Update()
    {
        if (!GameManager.CanUseKeyboard)
            return;
        UseDropActiveItemHandle();
        ChangeSlotHandle();
    }
    private void UseDropActiveItemHandle()
    {
        if (!_activeItem)
            return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            UseActiveItem();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            DropActiveItem();
        }
    }
    private void ChangeSlotHandle()
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
    }
    public bool PickUp(InventoryItem item, bool isUsable)
    {
        if (!isUsable)
        {
            item.Use();
            Destroy(item.gameObject);
            return false;
        }
        bool wasAdded = AddItem(item);
        if (wasAdded)
        {
            item.transform.SetParent(transform);
            item.transform.localPosition = new Vector3(0.1f, 0.5f, 0);
            OnAddition?.Invoke();
        }
        return wasAdded;
    }

    private void ChangeActiveSlot(int index)
    {
        if (_activeSlot == index)
        {
            ShutDownSlot();
            return;
        }
        if (InventoryItems[index - 1] == _emptyItem)
            return;
        if (_activeSlot != 0)
            ShutDownSlot();
        SetActiveSlot(index);
    }
    private void SetActiveSlot(int index)
    {
        _activeSlot = index;
        _activeItem = InventoryItems[index - 1];
        _activeItem.Show();
        if (_activeItem is IChargeableItem chargeableItem)
        {
            InventoryWin.FlashLightSliderAppear(chargeableItem);
        }
        ChangeAnimation();
    }
    public void ChangeAnimation()
    {
        if (_activeItem)
        {
            if (_activeItem is Flashlight)
            {
                PlayerAnimator.ChangeAnimState(PlayerAnimator.AnimStates.FlashLight);
            }
            else if (_activeItem is BigBob)
            {
                PlayerAnimator.ChangeAnimState(PlayerAnimator.AnimStates.BigBob);
            }
            else if (_activeItem is Candle)
            {
                PlayerAnimator.ChangeAnimState(PlayerAnimator.AnimStates.Candle);
            }
        }
        else
        {
            PlayerAnimator.ChangeAnimState(PlayerAnimator.AnimStates.Nothing);
        }
    }
    private void ShutDownSlot()
    {
        _activeItem.Hide();
        _activeSlot = 0;
        if (_activeItem is IChargeableItem)
        {
            InventoryWin.FlashLightSliderDisappear();
        }
        _activeItem.Hide();
        _activeItem = null;
        ChangeAnimation();
    }
    private bool AddItem(InventoryItem item)
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
        InventoryItems[_activeSlot - 1].Hide();
        CollectableItem colItem = InventoryItems[_activeSlot - 1].GetCollectableItem;
        colItem.transform.SetParent(RoomsManager.Instance.CurrentRoom.transform);
        colItem.transform.position = transform.position;
        colItem.ShowItem();
        DeleteItem(_activeSlot - 1);
        ShutDownSlot();
    }
    public void HideActiveItem()
    {
        if (_activeItem != null)
            _activeItem.Hide();
    }
    public void ShowActiveItem()
    {
        if (_activeItem != null)
            _activeItem.Show();
    }
    private void UseActiveItem()
    {
        var item = _activeItem;
        if (!_activeItem.CanKeep)
        {
            DropActiveItem();
        }
        item.Use();
    }
    public InventoryItem GetEmptyItem()
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
    public bool ActiveSlotIsBurningCandle()
    {
        var candle = _activeItem as Candle;
        if (candle == null) return false;

        var light = candle.GetComponent<Light2D>();
        if (light == null || !light.enabled) return false;

        return true;
    }

    public InventoryItem GetItem(int index)
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
