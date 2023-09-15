using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Inventory))]
public class Barter : MonoBehaviour
{
    [Header("Barter result UI")]
    [SerializeField] private BarterUI _barterUI;
    [Header("Inventory")]
    [SerializeField] private Inventory _inventoryNPC;
    
    private Inventory _inventoryPlayer;
    private int _totalCost;
    private List<Item> _itemsForSelling = new();

    public static UnityAction<Item, Transform> OnItemTransfer;
    public static UnityAction<Item, Transform> OnResetItemTransfer;

    private void Start()
    {
        _inventoryPlayer = GetComponent<InventoryPlayer>();

        OnItemTransfer += CalculateTotalCost;
        OnResetItemTransfer += DeductTotalCost;
        _barterUI.AcceptButton.onClick.AddListener(AcceptTransaction);
        _barterUI.CancelButton.onClick.AddListener(CancelTransaction);
    }

    private void CalculateTotalCost(Item item, Transform container)
    {    
        if (_itemsForSelling.Count == 0 || !_itemsForSelling.Contains(item))
        {
            _itemsForSelling.Add(item);
            Calculate(item, container);
            _barterUI.ActivateUI(_itemsForSelling.Count, _totalCost);
        }
    }

    private void DeductTotalCost(Item item, Transform container)
    {
        _itemsForSelling.Remove(item);
        Calculate(item, container);
        _barterUI.ActivateUI(_itemsForSelling.Count, _totalCost);
    }

    private void Calculate(Item item, Transform container)
    {
        if (container.TryGetComponent(out InventoryPlayer inventoryPlayer))
        {
            _totalCost -= item.Price * _inventoryNPC.MultiplierPriceItems;
        }
        else if (container.TryGetComponent(out InventoryNPC inventoryNPC))
        {
            _totalCost += item.Price;
        }

        _barterUI.RotationArrow(_totalCost);
    }

    private void AcceptTransaction()
    {
        if (!TryDebitMoney()) return;

        _totalCost = 0;
        foreach (var item in _itemsForSelling.ToList())
        {
            item.AcceptTrade();
            _itemsForSelling.Remove(item);
        }

        _barterUI.ActivateUI(_itemsForSelling.Count, _totalCost);
    }

    private void CancelTransaction()
    {
        _totalCost = 0;
        foreach (var item in _itemsForSelling.ToList())
        {
            item.Cancel();
            _itemsForSelling.Remove(item);
        }

        _barterUI.ActivateUI(_itemsForSelling.Count, _totalCost);
    }

    private bool TryDebitMoney()
    {
        if (_totalCost > 0 && _inventoryNPC.CurrentMoney >= _totalCost)
        {
            _inventoryPlayer.AddMoney(_totalCost);
            _inventoryNPC.RemoveMoney(_totalCost);
            return true;
        }
        else if (_totalCost < 0 && _inventoryPlayer.CurrentMoney >= -_totalCost)
        {
            _inventoryNPC.AddMoney(-_totalCost);
            _inventoryPlayer.RemoveMoney(-_totalCost);
            return true;
        }
        else if (_totalCost == 0 && _itemsForSelling.Count > 0)
            return true;
        else
            return false;
    }
}