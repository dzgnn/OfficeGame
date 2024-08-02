using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class TableData
{
    public bool isOpen;
    public int paperCount;
    public int deskIndex;
    public int sideMoney;
    public int unlockCost;

}

public class WorkerDesk : MonoBehaviour, IWorker, ILocked
{
    private GameData gamedata;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private UnityEngine.UI.Image costImage;
    [SerializeField] private Transform placePos;
    [SerializeField] private Transform moneyPos;
    [SerializeField] public List<GameObject> workPaperList = new List<GameObject>();
    private bool isPlacing;
    private float paperHeight = 0.05f;
    private Coroutine placeCoroutine;
    private Coroutine paperToMoneyCoroutine;

    [SerializeField] private int deskIndex;
    [SerializeField] private TableData tableData;
    [SerializeField] private GameObject openTab;
    [SerializeField] private GameObject closeTab;

    void Awake()
    {
        ActionManager.TakeMoneyAction += ZeroMoney;
    }



    void Start()
    {
        gamedata = JsonManager.gameData;

        if (JsonManager.dataFound)
        {
            TableData tableData = JsonManager.gameData.tables.Find(x => x.deskIndex == deskIndex);

            if (tableData is not null)
            {
                this.tableData = tableData;

                for (int i = 0; i < tableData.paperCount; i++)
                {
                    Vector3 paperPos = new Vector3(0, placePos.position.y + (i * paperHeight), 0);

                    GameObject paper = PoolManager.instance.GetPooledObject(PoolType.Paper, paperPos);
                    paper.transform.SetParent(placePos);
                    paper.transform.localPosition = new Vector3(0, i * paperHeight, 0);
                    workPaperList.Add(paper);
                }

                for (int i = 0; i < tableData.sideMoney; i++)
                {
                    PrefabManager.instance.CreateMoney(moneyPos.position);
                    tableData.sideMoney--;
                }
            }
            else
            {
                JsonManager.gameData.tables.Add(this.tableData);
                JsonManager.SaveData();
            }

        }
        else
        {
            JsonManager.gameData.tables.Add(this.tableData);
            JsonManager.SaveData();
        }

        if (tableData.isOpen)
        {
            closeTab.SetActive(false);
            openTab.SetActive(true);
        }
        else
        {
            closeTab.SetActive(true);
            openTab.SetActive(false);
        }


        if (tableData.paperCount > 0)
        {
            StartCoroutine(PaperToMoney());
        }

        UpdateUnlockCostUI();
    }

    public void PutPaper(Player player, List<GameObject> paperList)
    {
        if (!isPlacing && paperList.Count > 0)
        {
            isPlacing = true;
            placeCoroutine = StartCoroutine(PlacePapers(player, paperList));
        }

        if (paperToMoneyCoroutine == null)
        {
            paperToMoneyCoroutine = StartCoroutine(PaperToMoney());
        }
    }

    public void StopPlacing()
    {
        if (isPlacing)
        {
            isPlacing = false;
            if (placeCoroutine != null)
            {
                StopCoroutine(placeCoroutine);
            }
        }
    }

    private IEnumerator PlacePapers(Player player, List<GameObject> paperList)
    {
        while (paperList.Count > 0)
        {
            GameObject paper = paperList[0];
            paperList.RemoveAt(0);

            paper.transform.SetParent(placePos);
            workPaperList.Add(paper);
            tableData.paperCount++;
            JsonManager.SaveData();

            Vector3 paperPos = new Vector3(0, workPaperList.Count * paperHeight, 0);
            paper.transform.DOLocalMove(paperPos, 0.5f);

            yield return new WaitForSeconds(0.3f);
        }

        isPlacing = false;
        if (workPaperList.Count == 0 && paperToMoneyCoroutine != null)
        {
            StopCoroutine(paperToMoneyCoroutine);
            paperToMoneyCoroutine = null;
        }
    }

    private void ZeroMoney()
    {
        tableData.sideMoney = 0;
    }

    private IEnumerator PaperToMoney()
    {
        while (true)
        {
            if (workPaperList.Count > 0)
            {
                GameObject paper = workPaperList[workPaperList.Count - 1];

                paper.transform.DOScale(0, 1).OnComplete(() =>
                {
                    workPaperList.Remove(paper);
                    PoolManager.instance.DeactivatePooledObject(paper, PoolType.Paper);
                    paper.transform.DOScale(new Vector3(8, 8, 8), 0);
                    paper.transform.SetParent(null);

                    tableData.paperCount--;
                    tableData.sideMoney++;
                    JsonManager.SaveData();

                    Vector3 moneyTruePos = new Vector3(moneyPos.position.x, moneyPos.position.y, moneyPos.position.z);
                    PrefabManager.instance.CreateMoney(moneyTruePos);
                });

                yield return new WaitForSeconds(2.5f);
            }
            else
            {
                paperToMoneyCoroutine = null;
                yield break;
            }
        }
    }

    void ILocked.Unlock()
    {
        if (!tableData.isOpen)
        {
            StartCoroutine(SlowlyDecreaseMoney());
        }
    }


    private IEnumerator SlowlyDecreaseMoney()
    {
        while (tableData.unlockCost > 0 && GameManager.instance.GetMoney() > 0)
        {
            GameManager.instance.DecreaseMoney(1);
            tableData.unlockCost -= 1;
            JsonManager.SaveData();
            GameManager.instance.UpdateMoneyText();
            UpdateUnlockCostUI();

            yield return new WaitForSeconds(0.1f);
        }

        if (tableData.unlockCost == 0)
        {
            tableData.isOpen = true;
            closeTab.SetActive(false);
            openTab.SetActive(true);
        }
    }
    private void UpdateUnlockCostUI()
    {
        costText.text = tableData.unlockCost.ToString();
        costImage.fillAmount = 1 - Mathf.Clamp01((float)tableData.unlockCost / 20);
    }





}

