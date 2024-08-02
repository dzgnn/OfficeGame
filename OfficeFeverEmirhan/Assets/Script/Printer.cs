using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public class PrinterData
{
    public bool isOpen;
    public int paperCount;
    public int printerIndex;
}

public class Printer : MonoBehaviour, IPrinter, ILocked
{
    [SerializeField] private Transform deskPos;
    [SerializeField] private Transform createPoint;
    [SerializeField] List<GameObject> paperList = new List<GameObject>();

    private bool isCollecting;
    public float paperHeight = 0.25f;
    private Coroutine collectCoroutine;

    [SerializeField] private int printerIndex;
    [SerializeField] private PrinterData printerData;
    [SerializeField] private GameObject openTab;
    [SerializeField] private GameObject closeTab;

    private bool creatingPaper = true;

    void Start()
    {
        if (PoolManager.instance == null)
        {
            return;
        }

        if (JsonManager.dataFound)
        {
            PrinterData printerData = JsonManager.gameData.printers.Find(x => x.printerIndex == printerIndex);

            if (printerData is not null)
            {
                this.printerData = printerData;

                for (int i = 0; i < printerData.paperCount; i++)
                {
                    float paperCreatePosY = deskPos.position.y + (paperList.Count * paperHeight);
                    Vector3 paperPos = new Vector3(deskPos.position.x, paperCreatePosY, deskPos.position.z);

                    GameObject paper = PoolManager.instance.GetPooledObject(PoolType.Paper, paperPos);

                    paperList.Add(paper);
                }
            }
            else
            {
                JsonManager.gameData.printers.Add(this.printerData);
                JsonManager.SaveData();
            }

        }
        else
        {
            JsonManager.gameData.printers.Add(this.printerData);
            JsonManager.SaveData();
        }

        if (printerData.isOpen)
        {
            closeTab.SetActive(false);
            openTab.SetActive(true);
            StartCoroutine(CreatePaper());
        }
        else
        {
            closeTab.SetActive(true);
            openTab.SetActive(false);
        }


    }

    private IEnumerator CreatePaper()
    {
        while (creatingPaper)
        {

            if (paperList.Count >= 30 || isCollecting)
            {
                yield return new WaitForSeconds(1);
                continue;
            }

            float paperCreatePosY = deskPos.position.y + (paperList.Count * paperHeight);

            GameObject paper = PoolManager.instance.GetPooledObject(PoolType.Paper, createPoint.transform.position);
            paperList.Add(paper);
            paper.transform.rotation = new Quaternion(0, 0, 0, 0);
            printerData.paperCount++;

            Vector3 paperPos = new Vector3(deskPos.position.x, paperCreatePosY, deskPos.position.z);
            paper.transform.DOMove(paperPos, 1f);

            yield return new WaitForSeconds(1);
        }
    }

    public void DeactiveAllPapers()
    {
        foreach (GameObject paper in paperList)
        {
            PoolManager.instance.DeactivatePooledObject(paper, PoolType.Paper);
        }
        paperList.Clear();
    }

    public void Collect(Player player)
    {
        if (!isCollecting && paperList.Count > 0)
        {
            isCollecting = true;
            collectCoroutine = StartCoroutine(CollectPapers(player));

        }
    }

    public void StopCollecting()
    {
        if (isCollecting)
        {
            isCollecting = false;
            if (collectCoroutine != null)
            {
                StopCoroutine(collectCoroutine);
            }
        }
    }

    private IEnumerator CollectPapers(Player player)
    {
        while (paperList.Count > 0 && player.paperList.Count < player.handLimit)
        {
            GameObject paper = paperList[paperList.Count - 1];
            paperList.RemoveAt(paperList.Count - 1);

            paper.transform.SetParent(player.collectPos);
            player.paperList.Add(paper);
            printerData.paperCount--;

            Vector3 paperPos = new Vector3(0, player.paperList.Count * player.paperHeight, 0);
            paper.transform.DOLocalMove(paperPos, 1f).OnComplete(() =>
            {
                paper.transform.localRotation = Quaternion.identity;
            });

            yield return new WaitForSeconds(0.5f);
        }

        isCollecting = false;
    }

    void ILocked.Unlock()
    {
        if (printerData.isOpen == false)
        {
            if (GameManager.instance.TrySpendMoney(GameManager.instance.GetPrinterUnlockCost()))
            {
                printerData.isOpen = true;
                JsonManager.SaveData();
                closeTab.SetActive(false);
                openTab.SetActive(true);
                StartCoroutine(CreatePaper());
            }
            else
            {
                Debug.Log("Yeterli para yok!");
            }
        }
    }
}
