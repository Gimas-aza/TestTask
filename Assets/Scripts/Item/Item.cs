using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("Characteristics")]
    [SerializeField] private string _name;
    [SerializeField] private int _price;
    [SerializeField] private int _number;

    private Canvas _tradeMenuCanvas;
    private RectTransform _rectTransform;
    private Transform _parentTransform;
    private CanvasGroup _canvasGroup;
    private RectTransform _containerDragAndDrop;
    private Transform _container;
    private Inventory _inventory;
    private Slot _slot;
    private InformationItemUI _informationItemUI;

    public string Name => _name;
    public int Price => _price;
    public int Number => _number;
    public Transform Container => _container;

    public bool IsSelling = false;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _tradeMenuCanvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(RectTransform containerDragAndDrop, Transform containerForItem, InformationItemUI informationItemUI)
    {
        _containerDragAndDrop = containerDragAndDrop;
        _container = containerForItem;
        _informationItemUI = informationItemUI;
        _inventory = _container.GetComponent<Inventory>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _informationItemUI.OnShowInformation?.Invoke(this, _container);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _parentTransform = _rectTransform.parent;
        _parentTransform.SetParent(_containerDragAndDrop);
        _parentTransform.SetAsLastSibling();
        
        _canvasGroup.blocksRaycasts = false;
        _informationItemUI.OnShowInformation?.Invoke(this, _container);
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _tradeMenuCanvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _slot = _parentTransform.GetComponent<Slot>();
        transform.localPosition = Vector3.zero;
        _parentTransform.SetParent(_slot.ContainerItems);
        TryingResetSlot();

        _canvasGroup.blocksRaycasts = true;
        _informationItemUI.OnHiddenInformation?.Invoke();
    }

    public void AcceptTrade()
    {
        SetCurrentSlot();
        _inventory.RemoveItem(this);
        
        _container = _slot.ContainerItems;
        _inventory = _container.GetComponent<Inventory>();
        _inventory.AddItem(this);
        _slot.ResetColor();
        _slot.IsEmpty = false;
        IsSelling = false;
    }

    public void Cancel()
    {
        SetCurrentSlot();
        _slot.Reset();

        foreach (var slot in _inventory.Slots)
        {
            if (slot.IsEmpty)
            {
                _slot = slot;
                break;
            }
        }

        transform.SetParent(_slot.transform);
        transform.localPosition = Vector3.zero;
        _slot.IsEmpty = false;
        IsSelling = false;
    }

    private void SetCurrentSlot()
    {
        _parentTransform = _rectTransform.parent;
        _slot = _parentTransform.GetComponent<Slot>();
    }

    private void TryingResetSlot()
    {
        if (_rectTransform.parent != _parentTransform)
        {
            _slot.Reset();
        }
    }
}
