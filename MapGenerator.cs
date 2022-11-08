using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;

    [Range(0, 1)]
    public float obstaclePercent;

    private Coord startPoint;

    private void Start()
    {
        this.transform.position = Vector3.zero;
        GenerateMap();
        this.transform.position = new Vector3(0f, 1f, 0f);
    }

    public void GenerateMap()
    {
        List<Coord> allTileCoords;
        Queue<Coord> shuffledTileCoords;

        allTileCoords = new List<Coord>();
        for (float x = 0; x < mapSize.x; x ++)
        {
            for (float y = 0; y < mapSize.y; y ++)
            {
                allTileCoords.Add(new Coord(x, y));

                Vector2 tilePosition = CoordToPosition(allTileCoords.Last());
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, this.transform).transform;
            }
        }
        int seed = Random.Range(int.MinValue, int.MaxValue);
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleAtrray(allTileCoords.ToArray(), seed));
        startPoint = new Coord(1f, 1f);

        bool[,] obstacleMap = new bool[(int)mapSize.x, (int)mapSize.y];
        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObjstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord(shuffledTileCoords);
            obstacleMap[(int)randomCoord.x, (int)randomCoord.y] = true;
            currentObjstacleCount++;

            if (randomCoord != startPoint && MapIsFullyAccessible(obstacleMap, currentObjstacleCount))
            {
                Vector2 obstaclePosition = CoordToPosition(randomCoord);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition, Quaternion.identity, this.transform).transform;
            }
            else
            {
                obstacleMap[(int)randomCoord.x, (int)randomCoord.y] = false;
                currentObjstacleCount--;
            }
        }
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObjstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(startPoint);
        mapFlags[(int)startPoint.x, (int)startPoint.y] = true;

        int accessibleTileCount = 1;

        while (queue.Any())
        {
            Coord tile = queue.Dequeue();

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
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObjstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    private Vector2 CoordToPosition(Coord coord)
    {
        return new Vector2(-mapSize.x / 2f + 0.5f + coord.x, -mapSize.y / 2f + 0.5f + coord.y);
    }

    public Coord GetRandomCoord(Queue<Coord> shuffledTileCoords)
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public struct Coord
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
