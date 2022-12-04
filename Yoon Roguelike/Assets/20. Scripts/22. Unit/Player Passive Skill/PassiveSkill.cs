using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PassiveSkill : MonoBehaviour
{
    public string skillName;
    [Multiline]
    public string description;

    public int additionalAtk;
    public int additionaldefence;

    protected Unit unit;

    private void Awake()
    {
        unit = GetComponent<Unit>();
    }
}
