using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager instance { get; private set; } = null;

    [Header("Map")]
    public Transform tilePrefab;
    public Transform wallPrefab;
    public Vector2 mapSize;
    public Tile[,] tiles;
    public List<Tile> tilesList;

    [Header("Start, End Point")]
    public Transform startPointPrefab;
    public Transform nextPointPrefab;
    public Transform startPointPortal;
    public Transform nextPointPortal;
    public Vector2 startPoint;
    public Vector2 nextPoint;

    [Header("food")]
    public Transform foodHolder;
    public List<GameObject> foods;
    [Range(0, 1)]
    public float foodPercent;

    private PlayerController player;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        //GenerateMap(); // 순서 상 Awake에 있어야 함.
    }

    public void Init()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    public void ClearFood()
    {
        for(int i = 0; i < foodHolder.childCount; ++i)
        {
            Destroy(foodHolder.GetChild(i).gameObject);
        }
    }

    public Tile GetPlayerTile()
    {
        Vector3 playerPos = player.transform.position;
        return tiles[(int)playerPos.x, (int)playerPos.y];
    }

    public void GenerateMap(int seed, float obstaclePercent)
    {
        // 기존에 있던 타일 제거
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        List<Vector2> allTileCoords = new List<Vector2>(); ;
        Queue<Vector2> shuffledTileCoords;
        tiles = new Tile[(int)mapSize.x, (int)mapSize.y];

        #region 타일 생성
        tilesList = new List<Tile>();
        int tileIndex = 1;
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Vector2(x, y));

                Vector2 tilePosition = allTileCoords.Last();
                tiles[x, y] = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform).GetComponent<Tile>();
                tilesList.Add(tiles[x, y]);
                tiles[x, y].index = tileIndex++;
            }
        }
        shuffledTileCoords = new Queue<Vector2>(Utility.ShuffleAtrray(allTileCoords.ToArray(), seed));
        #endregion

        int wallCount = 0;
        #region 벽 생성
        bool[,] wallMap = new bool[(int)mapSize.x, (int)mapSize.y];
        wallCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentWallCount = 0;
        for (int i = 0; i < wallCount; i++)
        {
            Vector2 randomCoord = GetRandomVector2(shuffledTileCoords);
            wallMap[(int)randomCoord.x, (int)randomCoord.y] = true;
            currentWallCount++;

            if (randomCoord != startPoint && MapIsFullyAccessible(wallMap, currentWallCount))
            {
                Vector2 wallPosition = randomCoord;
                Tile newWall = Instantiate(wallPrefab, wallPosition, Quaternion.identity, transform).GetComponent<Tile>();
                Destroy(tiles[(int)wallPosition.x, (int)wallPosition.y].gameObject);
                tiles[(int)wallPosition.x, (int)wallPosition.y] = newWall;
            }
            else
            {
                wallMap[(int)randomCoord.x, (int)randomCoord.y] = false;
                currentWallCount--;
            }
        }

        for (int y = -1; y <= mapSize.y; y++)
        {
            Instantiate(wallPrefab, new Vector2(-1f, y), Quaternion.identity, this.transform);
            Instantiate(wallPrefab, new Vector2(mapSize.x, y), Quaternion.identity, this.transform);
        }
        for (int x = -1; x <= mapSize.x; x++)
        {
            Instantiate(wallPrefab, new Vector2(x, -1f), Quaternion.identity, this.transform);
            Instantiate(wallPrefab, new Vector2(x, mapSize.y), Quaternion.identity, this.transform);
        }
        #endregion

        #region 타일 간 연결
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                if (x - 1 >= 0)
                {
                    if (!tiles[x - 1, y].CompareTag("Wall"))
                    {
                        tiles[x, y].adjacent.Add(tiles[x - 1, y]);
                    }
                }

                if (x + 1 <= mapSize.x - 1)
                {
                    if (!tiles[x + 1, y].CompareTag("Wall"))
                    {
                        tiles[x, y].adjacent.Add(tiles[x + 1, y]);
                    }
                }
                if (y - 1 >= 0)
                {
                    if (!tiles[x, y - 1].CompareTag("Wall"))
                    {
                        tiles[x, y].adjacent.Add(tiles[x, y - 1]);
                    }
                }
                if (y + 1 <= mapSize.y - 1)
                {
                    if (!tiles[x, y + 1].CompareTag("Wall"))
                    {
                        tiles[x, y].adjacent.Add(tiles[x, y + 1]);
                    }
                }
            }
        }
        #endregion

        #region 시작포인트, 다음 레벨 입장포인트 생성
        int nowDistance = 1;
        Queue<Tile> q_Tiles = new Queue<Tile>();
        Queue<Tile> wantDistanceCheckTile = new Queue<Tile>();
        q_Tiles.Enqueue(tiles[(int)startPoint.x, (int)startPoint.y]);
        startPointPortal = Instantiate(startPointPrefab, startPoint, Quaternion.identity);
        while (q_Tiles.Any())
        {
            List<Tile> distanceLevel = q_Tiles.ToList();
            Tile tile = q_Tiles.Dequeue();
            
            foreach (Tile _tile in tile.adjacent)
            {
                if(!_tile.distanceCheck)
                {
                    _tile.distanceFromStartPoint = nowDistance;
                    _tile.distanceCheck = true;
                    wantDistanceCheckTile.Enqueue(_tile);
                }
            }

            if (!q_Tiles.Any())
            {
                if (!wantDistanceCheckTile.Any())
                {
                    Debug.Log(distanceLevel[0].position);
                    // end point 생성 후 while문 종료
                    nextPoint = new List<Tile>(Utility.ShuffleAtrray(distanceLevel.ToArray(), seed))[0].position;
                    nextPointPortal = Instantiate(nextPointPrefab, nextPoint, Quaternion.identity);

                    break;
                }

                while (wantDistanceCheckTile.Any())
                {
                    q_Tiles.Enqueue(wantDistanceCheckTile.Dequeue());
                }

                ++nowDistance;
            }
        }
        #endregion

        #region 음식 배치
        int foodCount = (int)((tilesList.Count - wallCount) * foodPercent);
        Debug.Log(foodCount);
        while (foodCount > 0)
        {
            Vector2 foodPosition = GetRandomVector2(shuffledTileCoords);
            if (tiles[(int)foodPosition.x, (int)foodPosition.y].CompareTag("Tile"))
            {
                --foodCount;
                Instantiate(foods[Random.Range(0, foods.Count)], foodPosition, Quaternion.identity, foodHolder);
            }
        }
        #endregion
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObjstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Vector2> queue = new Queue<Vector2>();
        queue.Enqueue(startPoint);
        mapFlags[(int)startPoint.x, (int)startPoint.y] = true;

        int accessibleTileCount = 1;

        while (queue.Any())
        {
            Vector2 tile = queue.Dequeue();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = (int)tile.x + x;
                    int neighbourY = (int)tile.y + y;

                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1))
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Vector2(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)mapSize.x * (int)mapSize.y - currentObjstacleCount;
        return targetAccessibleTileCount == accessibleTileCount;
    }

    public Vector2 GetRandomVector2(Queue<Vector2> shuffledTileCoords)
    {
        Vector2 randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Tile GetTileByVector3(Vector3 pos)
    {
        return tiles[(int)pos.x, (int)pos.y];
    }

    public Tile GetTileByTransform(Transform tr)
    {
        return GetTileByVector3(tr.position);
    }

#pragma warning disable CS0660 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.Equals(object o)를 재정의하지 않습니다.
#pragma warning disable CS0661 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.GetHashCode()를 재정의하지 않습니다.
    public struct Coord
#pragma warning restore CS0661 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.GetHashCode()를 재정의하지 않습니다.
#pragma warning restore CS0660 // 형식은 == 연산자 또는 != 연산자를 정의하지만 Object.Equals(object o)를 재정의하지 않습니다.
    {
        public float x;
        public float y;

        public Coord(float _x, float _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator == (Coord a, Coord b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator != (Coord a, Coord b)
        {
            return !(a == b);
        }
    }
}
