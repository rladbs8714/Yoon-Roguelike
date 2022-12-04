using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Unit
{
    private Unit target;

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Unit>();
        turnOtherController.Action += new TurnOtherController.TurnOtherAction(Turn);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            turnOtherController.Action();
        }

        if (!isActioning && turnCount <= 0)
        {
            TurnEnd();
        }
    }

    public Tile GetNextTileByBfs()
    {
        int x = (int)this.transform.position.x;
        int y = (int)this.transform.position.y;

        Queue<Tile> q = new Queue<Tile>();
        List<int> floorVisitedCheck = new List<int>();
        Dictionary<Tile, Tile> node = new Dictionary<Tile, Tile>();
        bool find = false;

        q.Enqueue(MapManager.instance.tiles[x, y]);
        floorVisitedCheck.Add(MapManager.instance.tiles[x, y].index);
        node.Add(MapManager.instance.tiles[x, y], null);

        Tile nowTile = null;
        while (q.Any())
        {
            nowTile = q.Dequeue();
            if (nowTile.unit == target)
            {
                find = true;
                break;
            }

            foreach (Tile tile in nowTile.adjacent)
            {
                if (!(floorVisitedCheck.Find(i => i == tile.index) == tile.index) && !tile.onEnemy)
                {
                    node.Add(tile, nowTile);
                    q.Enqueue(tile);
                    floorVisitedCheck.Add(tile.index);
                }
            }
        }

        Stack<Tile> root = new Stack<Tile>();
        while (find)
        {
            if (node[nowTile] == null)
            {
                break;
            }
            root.Push(nowTile);
            nowTile = node[nowTile];
        }

        if (root.Any() && find)
        {
            return root.Pop();
        }
        else
        {
            return this.nowTile;
        }
    }

    public void Turn()
    {
        Tile nextTile = GetNextTileByBfs();

        if (nextTile.onPlayer)
        {
            Attack(target.transform.position);
        }
        else
        {
            Move(nextTile);
        }

        turnCount--;
    }

    private void Move(Tile nextTile)
    {
        StartCoroutine(Movement(nextTile.transform.position));
    }
}
