using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    private Item _item;

    public bool IsEmpty { get; private set; } = true;

    public void OnDrop(PointerEventData eventData)
    {
        var otherItemTransform = eventData.pointerDrag.transform;
        if (!otherItemTransform.TryGetComponent(out _item)) return;

        otherItemTransform.SetParent(transform);
        otherItemTransform.localPosition = Vector3.zero;
        IsEmpty = false;
    }
}
