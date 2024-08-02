using System.Collections.Generic;
using System.IO;
using UnityEngine;


[System.Serializable]
public class GameData
{
    public int money;
    public int printerUnlockCost = 20;
    //public int deskUnlockCost = 20;
    public List<TableData> tables = new List<TableData>();
    public List<PrinterData> printers = new List<PrinterData>();
}

public static class JsonManager
{
    public static GameData gameData;
    public static GameData defaultData = new GameData
    {
        money = 0,
        printerUnlockCost = 20,
        //deskUnlockCost = 20,
        printers = new List<PrinterData>(),
        tables = new List<TableData>()
    };

    private static string filePath = Application.dataPath + "/data.json";
    public static bool dataFound;

    public static void SaveData()
    {
        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(filePath, json);
    }

    public static void ResetData()
    {
        gameData = defaultData;
        SaveData();
    }

    public static void LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            dataFound = true;
        }
        else
        {
            dataFound = false;
            gameData = new GameData();
            SaveData();
        }
    }
}

