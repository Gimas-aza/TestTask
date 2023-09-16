using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Inventory : MonoBehaviour
{
    [SerializeField] private int _currentMoney;
    [Header("Slot")]
    [SerializeField] private Slot _slotPrefab;
    [SerializeField] private int _numberSlots;
    [Header("Items")]
    [SerializeField] private List<Item> _items = new();
    [SerializeField] private InformationItemUI _informationItemUI;
    [Header("Drag and Drop Container")]
    [SerializeField] private RectTransform _containerDragAndDrop;
    [Header("Money")]
    [SerializeField] private BarMoneyUI _barMoneyUI;
    [SerializeField] private int _multiplierPriceItems;


    private List<Slot> _slots = new(); 
    private GridLayoutGroup _containerItems;
    private float _deactivationDelay = 0.01f;

    public int CurrentMoney => _currentMoney;
    public int MultiplierPriceItems => _multiplierPriceItems;
    public List<Slot> Slots => _slots;
    public List<Item> Items => _items;

    private void Awake()
    {
        _containerItems = GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        CreateSlots();
        CreateItems();

        _barMoneyUI.SetMoney(_currentMoney);
    }

    protected void CreateSlots()
    {
        for (int i = 0; i < _numberSlots; i++)
        {
            var instantiateSlot = Instantiate(_slotPrefab, _containerItems.transform);
            instantiateSlot.Init(_containerItems.transform);
            _slots.Add(instantiateSlot);
        }

        Invoke(nameof(DeactivateGrid), _deactivationDelay);
    }

    private void DeactivateGrid()
    {
        _containerItems.enabled = false;
    }

    protected void CreateItems()
    {
        for (int i = 0; i < _items.Count; i++)
        {
            for (int j = 0; j < _slots.Count; j++)
            {
                if (_slots[j].IsEmpty)
                {
                    var instantiateItem = Instantiate(_items[i], _slots[j].transform);
                    instantiateItem.Init(_containerDragAndDrop, _containerItems.transform, _informationItemUI);
                    _items[i] = instantiateItem;
                    _slots[j].IsEmpty = false;
                    break;
                }
            }
        }
    }

    public void AddMoney(int money)
    {
        _currentMoney += money;
        _barMoneyUI.SetMoney(_currentMoney);
    }

    public void RemoveMoney(int money)
    {
        _currentMoney -= money;
        _barMoneyUI.SetMoney(_currentMoney);
    }

    public void AddItem(Item item)
    {
        _items.Add(item);
    }

    public void RemoveItem(Item item)
    {
        _items.Remove(item);
    }
}
