

using System.Collections.Generic;
using UnityEngine;

public interface IPrinter
{
   void Collect(Player player);
   void StopCollecting();
}


public interface IWorker
{
   void PutPaper(Player player, List<GameObject> paperList);
   void StopPlacing();
}


public interface IMoney
{
   void TakeMoney();
}

public interface ILocked
{
   void Unlock();
}