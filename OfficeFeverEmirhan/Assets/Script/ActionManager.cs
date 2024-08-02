using System;
using System.Reflection;

public class ActionManager
{
    public static Action TakeMoneyAction;

    public static void ClearActionManagerData()
    {
        var info = typeof(ActionManager)
        .GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var item in info)
        {
            item.SetValue(item.Name, null);
        }
    }

}