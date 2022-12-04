using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawnManager : MonoBehaviour
{
    public static UnitSpawnManager instance;

    public Transform unitHolder;
    public Transform enemyHolder;

    public List<Unit> units = new List<Unit>();

    [Header("Player")]
    public int playerIndex;
    public List<GameObject> playerPrefabs;
    public Unit player;

    [Header("Enemy")]
    public List<GameObject> enemyPrefabs;
    [Range(0, 1)]
    public float enemySpawnPercent;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        playerIndex = GameManager.instance.playerIndex;
    }

    public void PlayerSpawn(Vector2 startPosition)
    {
        player = Instantiate(playerPrefabs[playerIndex], startPosition, Quaternion.identity, unitHolder).GetComponent<Unit>();
        player.UnitPlacement(startPosition);
    }

    public void EnemySpawn(int roundLevel, Tile spawnTile)
    {
        Unit enemy = Instantiate(GetEnemy(roundLevel), spawnTile.position, Quaternion.identity, enemyHolder).GetComponent<Unit>();
        enemy.UnitPlacement(spawnTile.position);
        units.Add(enemy);
    }

    public GameObject GetEnemy(int roundLevel)
    {
        int newSpawnEnemyIndex = Random.Range(roundLevel - 1, roundLevel + 1);
        newSpawnEnemyIndex = MinMaxCheck(newSpawnEnemyIndex, 0, enemyPrefabs.Count);
        return enemyPrefabs[newSpawnEnemyIndex];
    }

    private int MinMaxCheck(int n, int min, int max)
    {
        if (n < min)
        {
            return min;
        }
        else if (n > max)
        {
            return max;
        }

        return n;
    }
}
