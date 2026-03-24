using UnityEngine;
using UnityEngine.UI;

public class InventoryWindow : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Image[] _inventoryIcons;
    [SerializeField] private Sprite[] _inventoryBG;
    [SerializeField] private Slider _flashLightSlider;
    private FlashlightItem _flashlightItem;
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
            if (item == _inventory.GetEmptyItem())
                _inventoryIcons[i].gameObject.SetActive(false);
            else
            {
                _inventoryIcons[i].sprite = item.GetIcon();
                _inventoryIcons[i].gameObject.SetActive(true);
            }
        }
    }
    private void Update()
    {
        if (_flashLightSlider.IsActive())
            _flashLightSlider.value = _flashlightItem.FlaslightCharge;
    }
    public void FlashLightSliderAppear(FlashlightItem flashlight)
    {
        _flashlightItem = flashlight;
        _flashLightSlider.gameObject.SetActive(true);
        _flashLightSlider.value = _flashlightItem.FlaslightCharge;
    }
    public void FlashLightSliderDisappear()
    {
        _flashlightItem = null;
        _flashLightSlider.gameObject.SetActive(false);
        _flashLightSlider.value = 0;
    }
}
