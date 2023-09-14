using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private string _name;
    [SerializeField] private int _price;
    [SerializeField] private int _number;

    private Canvas _tradeMenuCanvas;
    private RectTransform _rectTransform;
    private Transform _parentTransform;
    private CanvasGroup _canvasGroup;
    private RectTransform _containerDragAndDrop;
    private Transform _container;
    private Slot _slot;

    public string Name => _name;
    public int Price => _price;
    public int Number => _number;
    public Transform Container => _container;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _tradeMenuCanvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init(RectTransform containerDragAndDrop, Transform containerForItem)
    {
        _containerDragAndDrop = containerDragAndDrop;
        _container = containerForItem;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _parentTransform = _rectTransform.parent;
        _parentTransform.SetParent(_containerDragAndDrop);
        _parentTransform.SetAsLastSibling();
        _canvasGroup.blocksRaycasts = false;
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
        TryingResetColor();

        _canvasGroup.blocksRaycasts = true;
    }

    private void TryingResetColor()
    {
        if (_rectTransform.parent != _parentTransform)
        {
            _slot.ResetColor();
            _slot.IsEmpty = true;
        }
    }
}
