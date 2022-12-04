using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOtherController : MonoBehaviour
{
    public bool isLast;

    public delegate void TurnOtherAction();
    public TurnOtherAction Action = null;

    private Unit unit;

    private void Start()
    {
        unit = GetComponent<Unit>();
    }

    public void TurnStart()
    {
        if(Action != null)
        {
            Action();
        }
    }

    public void TurnEnd()
    {
        TurnManager.instance.TurnEnd(this);
    }
}
