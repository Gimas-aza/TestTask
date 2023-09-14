using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IDropHandler
{
    private Item _item;
    private Transform _containerItems;
    private Image _image;
    private Barter _barter;

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
            Barter.OnItemTransfer?.Invoke(_item);
        }
        else
        {
            Barter.OnResetItemTransfer?.Invoke(_item);
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
}
