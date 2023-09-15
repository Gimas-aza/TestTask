using UnityEngine;
using UnityEngine.UI;

public class BarMoneyUI : MonoBehaviour
{
    [SerializeField] private Text _moneyText;

    public void SetMoney(int money)
    {
        _moneyText.text = money.ToString();
    }
}
