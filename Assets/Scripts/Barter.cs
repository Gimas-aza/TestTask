using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Inventory))]
public class Barter : MonoBehaviour
{
    [SerializeField] private Inventory _inventoryNPC;
    
    private Inventory _inventoryPlayer;
    private int _totalCost;

    public static UnityAction<Item> OnItemTransfer;
    public static UnityAction<Item> OnResetItemTransfer;

    private void Start()
    {
        _inventoryPlayer = GetComponent<Inventory>();
        OnItemTransfer += CalculatingPurchasePrice;
        OnResetItemTransfer += DeductiblePurchasePrice;
    }

    private void CalculatingPurchasePrice(Item item)
    {
        _totalCost += item.Price;
        Debug.Log(_totalCost);
    }

    private void DisplayPurchaseConfirmation(Item item)
    {
        
    }

    private void DeductiblePurchasePrice(Item item)
    {
        _totalCost -= item.Price;
        Debug.Log(_totalCost);
    }
}