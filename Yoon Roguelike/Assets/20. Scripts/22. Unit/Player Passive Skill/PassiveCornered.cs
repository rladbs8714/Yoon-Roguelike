using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveCornered : PassiveSkill
{
    [Space(10)]
    [SerializeField]
    private int adjacentWallCount = 0;

    public void Update()
    {
        adjacentWallCount = 4 - unit.nowTile.adjacent.Count;

        additionalAtk = adjacentWallCount;
        additionaldefence = adjacentWallCount;
    }
}
