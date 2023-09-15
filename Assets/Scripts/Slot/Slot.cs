using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler
{
    private Item _item;
    private Transform _containerItems;
    private Image _image;

    public bool IsEmpty = true;
    public Transform ContainerItems => _containerItems;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Init(Transform containerItems)
    {
        _containerItems = containerItems;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var otherItemTransform = eventData.pointerDrag.transform;

        if (otherItemTransform.TryGetComponent(out _item) && IsEmpty)
        {
            CheckItemTransferToOtherInventory();
            otherItemTransform.SetParent(transform);
            otherItemTransform.localPosition = Vector3.zero;
            IsEmpty = false;
        }
    }

    private void CheckItemTransferToOtherInventory()
    {
        if (_containerItems != _item.Container)
        {
            SetColor();
            _item.IsSelling = true;
            Barter.OnItemTransfer?.Invoke(_item, _containerItems);
        }
        else if (_item.IsSelling)
        {
            _item.IsSelling = false;
            Barter.OnResetItemTransfer?.Invoke(_item, _containerItems);
        }
    }

    private void SetColor()
    {
        _image.color = new Color32(255, 240, 0, 255);
    }

    public void ResetColor()
    {
        _image.color = new Color32(255, 255, 255, 255);
    }

    public void Reset()
    {
        ResetColor();
        IsEmpty = true;
    }
}
