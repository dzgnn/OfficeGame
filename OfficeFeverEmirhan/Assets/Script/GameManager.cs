using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private GameData gameData;

    private void Awake()
    {
        instance = this;
        ActionManager.TakeMoneyAction += UpdateMoney;
        JsonManager.LoadData();
    }

    private void Start()
    {
        gameData = JsonManager.gameData;
        UpdateMoneyText();
    }

    [Button]
    public void ResetData()
    {
        JsonManager.ResetData();
    }

    private void UpdateMoney()
    {
        gameData.money += 1;
        JsonManager.SaveData();
        UpdateMoneyText();
    }

    public bool TrySpendMoney(int amount)
    {
        if (gameData.money >= amount)
        {
            gameData.money -= amount;
            JsonManager.SaveData();
            UpdateMoneyText();
            return true;
        }

        return false;
    }

    public void SpendMoney(int amount)
    {
        if (amount > gameData.money)
        {
            gameData.money = 0;
        }
        else
        {
            gameData.money -= amount;
        }
    }

    public int GetPrinterUnlockCost()
    {
        return gameData.printerUnlockCost;
    }

    public int GetMoney()
    {
        return gameData.money;
    }

    public void UpdateMoneyText()
    {
        CanvasManager.Instance.moneyText.text = gameData.money.ToString();
    }

    public void DecreaseMoney(int amount)
    {
        gameData.money = Mathf.Max(0, gameData.money - amount);
    }

}
