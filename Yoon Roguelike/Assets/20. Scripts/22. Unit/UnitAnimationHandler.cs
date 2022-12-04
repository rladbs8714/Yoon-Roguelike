using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationHandler : MonoBehaviour
{
    private Unit unit;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public void PlayerDieAnimationAfter()
    {
        DieAnimationAfter();
    }

    public void DieAnimationAfter()
    {
        Destroy(gameObject);
    }
}
