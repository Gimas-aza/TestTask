using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InformationItemUI : MonoBehaviour
{
    [SerializeField] private Text _name;
    [SerializeField] private Text _price;

    public UnityAction<Item, Transform> OnShowInformation;
    public UnityAction OnHiddenInformation;

    private void Start()
    {
        OnShowInformation += SetInformation;
        OnHiddenInformation += Deactivate;
        Deactivate();
    }

    private void SetInformation(Item item, Transform itemContainer)
    {
        if (itemContainer.TryGetComponent(out Inventory inventory))
        {
            _name.text = item.Name;
            _price.text = (item.Price * inventory.MultiplierPriceItems).ToString();
        }
        gameObject.SetActive(true);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
