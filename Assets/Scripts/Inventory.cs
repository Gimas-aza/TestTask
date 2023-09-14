using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int _money;
    [Header("Slot")]
    [SerializeField] private Slot _slotPrefab;
    [SerializeField] private int _numberSlots;
    [Header("Items")]
    [SerializeField] private List<Item> _items = new();
    [Header("Drag and Drop Container")]
    [SerializeField] private RectTransform _containerDragAndDrop;

    private List<Slot> _slots = new(); 
    private GridLayoutGroup _containerItems;

    public int Money => _money;
    public List<Slot> Slots => _slots;

    private void Awake()
    {
        _containerItems = GetComponent<GridLayoutGroup>();
    }

    private void Start()
    {
        CreateSlots();
        CreateItems();
    }

    private void CreateSlots()
    {
        for (int i = 0; i < _numberSlots; i++)
        {
            var instantiateSlot = Instantiate(_slotPrefab, _containerItems.transform);
            instantiateSlot.Init(_containerItems.transform);
            _slots.Add(instantiateSlot);
        }

        Invoke(nameof(DeactivateGrid), 0.1f);
    }

    private void DeactivateGrid()
    {
        _containerItems.enabled = false;
    }

    private void CreateItems()
    {
        int i = 0;
        int j = 0;

        for (; i < _items.Count; i++)
        {
            for (; j < _slots.Count; j++)
                if (_slots[j].IsEmpty) break;

            var instantiateItem = Instantiate(_items[i], _slots[j].transform);
            instantiateItem.Init(_containerDragAndDrop, _containerItems.transform); // todo Менять _containerItems.transform при продажи
            _slots[j].IsEmpty = false;
        }
    }

    public void AddMoney(int money)
    {
        _money += money;
        Debug.Log(_money);
    }

    public void RemoveMoney(int money)
    {
        _money -= money;
        Debug.Log(_money);
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
