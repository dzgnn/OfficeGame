using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    [SerializeField] private GameObject moneyPrefab;
    [SerializeField] private GameObject printerPrefab;
    [SerializeField] private GameObject deskPrefab;
    public static PrefabManager instance;
    void Awake()
    {
        instance = this;
    }

    public GameObject CreateMoney(Vector3 moneyPos)
    {
        return Instantiate(moneyPrefab, moneyPos, Quaternion.identity);
    }

    public GameObject CreatePrinter(Vector3 printerPos)
    {
        return Instantiate(printerPrefab, printerPos, Quaternion.identity);
    }

    public GameObject CreateDesk(Vector3 printerPos, Transform parent = null)
    {
        Quaternion deskTurn = Quaternion.Euler(0, 90, 0);
        GameObject desk = Instantiate(deskPrefab, printerPos, deskTurn);
        if (parent != null)
        {
            desk.transform.SetParent(parent);
        }
        return desk;
    }


    

}
