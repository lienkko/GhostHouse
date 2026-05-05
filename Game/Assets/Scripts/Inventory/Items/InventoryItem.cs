using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private Sprite _icon;
    public string Name => _name;
    public Sprite GetIcon => _icon;

    public void TakeInHand()
    {
        Debug.Log("Предмет в руках");
    }

    public void Hide()
    {
        Debug.Log("Предмет не в руках");
    }
}
