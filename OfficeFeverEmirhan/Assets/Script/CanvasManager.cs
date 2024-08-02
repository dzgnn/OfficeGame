using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI moneyText;
    public static CanvasManager Instance;
    void Awake()
    {
        Instance = this;
    }

    public void UpdateMoneyText(int money)
    {
        moneyText.text = money.ToString();
    }
}
