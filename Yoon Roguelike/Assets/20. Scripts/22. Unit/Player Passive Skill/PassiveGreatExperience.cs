using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveGreatExperience : PassiveSkill
{
    [Space(10)]
    [SerializeField]
    private int maxKillStack = 0;

    public void Update()
    {
        additionalAtk = unit.killCount <= maxKillStack ? unit.killCount : maxKillStack;
    }
}