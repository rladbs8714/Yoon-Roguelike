using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager instance;

    public int roundLevel = 0;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        // init
        roundLevel = 0;
 
        UnitSpawnManager.instance.unitHolder = GameObject.Find("Units").transform;
        UnitSpawnManager.instance.enemyHolder = GameObject.Find("Enemys").transform;

        RoundStart();
    }

    public void RoundStart()
    {
        ++roundLevel;

        // ���� �ʱ�ȭ
        MapManager.instance.ClearFood();

        // ������ ��Ż ����
        if (MapManager.instance.startPointPortal)
        {
            Destroy(MapManager.instance.startPointPortal.gameObject);
        }
        if (MapManager.instance.nextPointPortal)
        {
            Destroy(MapManager.instance.nextPointPortal.gameObject);
        }

        // �� ����
        MapManager.instance.GenerateMap(Random.Range(0, int.MaxValue), Random.Range(0.3f, 0.5f));

        // �÷��̾ ������ ������ ���� ����
        for (int i = 1; i < UnitSpawnManager.instance.units.Count; ++i)
        {
            Destroy(UnitSpawnManager.instance.units[i].gameObject);
        }
        UnitSpawnManager.instance.units.Clear();

        // �÷��̾ �������� �ʴٸ� (���� ������ �����̶��) �÷��̾ �����ϰ�, �ƴ϶�� ���� ����Ʈ�� ��ġ ����
        if (UnitSpawnManager.instance.player == null)
        {
            UnitSpawnManager.instance.PlayerSpawn(MapManager.instance.startPoint);

            GameUIManager.instance.Init();
            MapManager.instance.Init();
        }
        else
        {
            UnitSpawnManager.instance.player.UnitPlacement(MapManager.instance.startPoint);
        }
        UnitSpawnManager.instance.units.Add(UnitSpawnManager.instance.player);

        // UI �ʱ�ȭ
        GameUIManager.instance.Init();

        // �� ����
        List<Tile> spawneTile = MapManager.instance.tilesList.FindAll(t => t.distanceFromStartPoint > 5);
        spawneTile = new List<Tile>(Utility.ShuffleAtrray(spawneTile.ToArray(), Random.Range(0, int.MaxValue)));
        int enemyCount = (int)(MapManager.instance.tilesList.Count * UnitSpawnManager.instance.enemySpawnPercent);
        for (int i = 0; i < enemyCount; ++i)
        {
            UnitSpawnManager.instance.EnemySpawn(roundLevel, spawneTile[i]);
        }

        TurnManager.instance.RoundStart(UnitSpawnManager.instance.units);
    }

    public void NextRound()
    {
        RoundStart();
    }
}
