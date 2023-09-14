using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Inventory))]
public class Barter : MonoBehaviour
{
    [Header("Barter result UI")]
    [SerializeField] private GameObject _barterResultUI;
    [SerializeField] private Image _arrow;
    [SerializeField] private Text _totalCostText;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _cancelButton;
    [Header("Inventory")]
    [SerializeField] private InventoryNPC _inventoryNPC;
    
    private InventoryPlayer _inventoryPlayer;
    private int _totalCost;
    private List<Item> _itemsForSelling = new();

    public static UnityAction<Item, Transform> OnItemTransfer;
    public static UnityAction<Item, Transform> OnResetItemTransfer;

    private void Start()
    {
        _inventoryPlayer = GetComponent<InventoryPlayer>();

        OnItemTransfer += CalculatingTotalCost;
        OnResetItemTransfer += DeductibleTotalCost;
        _acceptButton.onClick.AddListener(AcceptTransaction);
        _cancelButton.onClick.AddListener(CancelTransaction);
    }

    private void CalculatingTotalCost(Item item, Transform container)
    {
        List<Item> itemFind = _itemsForSelling.FindAll(x => x == item);
        
        if (_itemsForSelling.Count == 0 || itemFind.Count == 0)
        {
            _itemsForSelling.Add(item);
            Calculating(item, container);

            ActivateBarterResultUI();
        }
    }

    private void DeductibleTotalCost(Item item, Transform container)
    {
        _itemsForSelling.Remove(item);
        Calculating(item, container);

        ActivateBarterResultUI();
    }

    private void Calculating(Item item, Transform container)
    {
        if (container.TryGetComponent(out InventoryPlayer inventoryPlayer))
        {
            _totalCost -= item.Price;

            if (_totalCost < 0)
                _arrow.transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        else if (container.TryGetComponent(out InventoryNPC inventoryNPC))
        {
            _totalCost += item.Price;

            if (_totalCost > 0)
                _arrow.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
    }

    private void ActivateBarterResultUI()
    {
        if (_itemsForSelling.Count == 0 && _totalCost == 0)
            _barterResultUI.SetActive(false);
        else
            _barterResultUI.SetActive(true);
        _totalCostText.text = _totalCost.ToString();
    }

    private void AcceptTransaction()
    {
        if (_totalCost > 0 && _inventoryNPC.Money >= _totalCost)
        {
            _inventoryPlayer.AddMoney(_totalCost);
            _inventoryNPC.RemoveMoney(_totalCost);
        }
        else if (_totalCost < 0 && _inventoryPlayer.Money >= -_totalCost)
        {
            _inventoryNPC.AddMoney(-_totalCost);
            _inventoryPlayer.RemoveMoney(-_totalCost);
        }
        else
            return;
        
        _totalCost = 0;
        ActivateBarterResultUI();

        foreach (var item in _itemsForSelling.ToList())
        {
            item.Save();

            _itemsForSelling.Remove(item);
        }
    }

    private void CancelTransaction()
    {
        foreach (var item in _itemsForSelling.ToList())
        {
            item.Cancel();

            _itemsForSelling.Remove(item);
        }
    }
}