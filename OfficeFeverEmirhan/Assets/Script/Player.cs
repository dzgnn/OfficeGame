using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public List<GameObject> paperList = new List<GameObject>();
    public Transform collectPos;
    public int handLimit = 10;
    public float paperHeight = 0.15f;

    private IPrinter currentPrinter;
    private IWorker currentWorker;


    private void OnTriggerEnter(Collider other)
    {
        IPrinter printer = other.gameObject.GetComponent<IPrinter>();
        if (printer is not null)
        {
            currentPrinter = printer;
            currentPrinter.Collect(this);
        }


        IWorker worker = other.gameObject.GetComponent<IWorker>();
        if (worker is not null)
        {
            currentWorker = worker;
            currentWorker.PutPaper(this, paperList);
        }


        other.gameObject.GetComponent<IMoney>()?.TakeMoney();
        other.gameObject.GetComponent<ILocked>()?.Unlock();


    }

    private void OnTriggerExit(Collider other)
    {
        if (currentPrinter != null && other.gameObject.GetComponent<IPrinter>() == currentPrinter)
        {
            currentPrinter.StopCollecting();
            currentPrinter = null;
        }

        if (currentWorker != null && other.gameObject.GetComponent<IWorker>() == currentWorker)
        {
            currentWorker.StopPlacing();
            currentWorker = null;
        }
    }


}
