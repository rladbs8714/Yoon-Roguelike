using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool distanceCheck = false;
    public bool onEnemy = false;
    public bool onPlayer = false;
    public int index;
    public int distanceFromStartPoint = 0;

    public Unit unit = null;

    public Vector3 position
    {
        get { return transform.position; }
    }
    public List<Tile> adjacent = new List<Tile>();

    public void OnUnit(Unit unit)
    {
        if (unit.CompareTag("Player"))
        {
            onPlayer = true;
        }
        else if (unit.CompareTag("Enemy"))
        {
            onEnemy = true;
        }
        this.unit = unit;
    }

    public void ExitUnit()
    {
        onEnemy = false;
        onPlayer = false;
        unit = null;
    }
}
