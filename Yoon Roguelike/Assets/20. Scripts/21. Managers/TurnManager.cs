using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance = null;
    public LinkedList<TurnOtherController> turnOtherControllers = new LinkedList<TurnOtherController>();

    private TurnOtherController turnOtherController = null;
    private TurnOtherController previousTurnOtherController = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if((previousTurnOtherController != turnOtherController) && turnOtherController)
        {
            previousTurnOtherController = turnOtherController;
            turnOtherController.TurnStart();
        }
    }

    public void RoundStart(List<Unit> units)
    {
        turnOtherControllers.Clear();

        foreach(Unit unit in units)
        {
            turnOtherControllers.AddLast(unit.GetComponent<TurnOtherController>());
        }
        turnOtherControllers.Last().isLast = true;

        turnOtherControllers.First().TurnStart();
    }

    public void TurnEnd(TurnOtherController toc)
    {
        if(!toc.isLast)
        {
            turnOtherController = turnOtherControllers.Find(toc).Next.Value;
        }
        else
        {
            turnOtherController = turnOtherControllers.First();

            GameUIManager.instance.Init();
        }
    }
}
