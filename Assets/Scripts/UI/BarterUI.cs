using UnityEngine;
using UnityEngine.UI;

public class BarterUI : MonoBehaviour
{
    [SerializeField] private Image _arrow;
    [SerializeField] private Text _totalCostText;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _cancelButton;

    public Button AcceptButton => _acceptButton;
    public Button CancelButton => _cancelButton;

    public void ActivateUI(int numberItemsForSelling, int totalCost)
    {
        if (numberItemsForSelling == 0 && totalCost == 0)
            gameObject.SetActive(false);
        else
            gameObject.SetActive(true);
        _totalCostText.text = totalCost.ToString();
    }

    public void RotationArrow(int totalCost)
    {
        if (totalCost < 0)
            _arrow.transform.localEulerAngles = new Vector3(0, 0, 0);
        else if (totalCost > 0)
            _arrow.transform.localEulerAngles = new Vector3(0, 180, 0);
    }
}
