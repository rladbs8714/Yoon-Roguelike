using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public int level = 0;

    public Transform unitHolder;
    public List<Unit> units;

    [Header("Player")]
    public int playerIndex;
    public List<GameObject> playerList;
    public Unit player;

    [Header("Enemys")]
    public Transform enemyHolder;
    public List<GameObject> enemyPrefabs;
    [Range(0, 1)]
    public float enemyPercent;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
