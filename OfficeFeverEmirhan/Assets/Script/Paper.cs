using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paper : MonoBehaviour, IMoney
{
    public void TakeMoney()
    {
        ActionManager.TakeMoneyAction?.Invoke();
        Destroy(gameObject);
    }
}
