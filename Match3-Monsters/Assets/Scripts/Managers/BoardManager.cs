using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class BoardManager
{
    public static Vector2 boardRoot = new Vector2(-96f, -96f);

    public int boardWidth = 5;
    public int boardHeight = 5;
    public const float boardStep = 48f;

    public Board Board { get; private set; }
    public MonsterBlock MonsterPrefab { get; private set; }
    public MonsterType[] MonsterTypes { get; private set; }
    public Transform BoardParent { get; private set; }

    public BoardManager(MonsterBlock prefab, MonsterType[] monsterTypes, Transform parent)
    {
        Board = new Board(new Tile[boardWidth, boardHeight]);
        MonsterPrefab = prefab;
        MonsterTypes = monsterTypes;
        BoardParent = parent;
    }

    public static Vector2Int CoordsToVectorInt(Vector2 coord)
    {
        int xCoord = Mathf.RoundToInt((coord.x - boardRoot.x) / boardStep);
        int yCoord = Mathf.RoundToInt((coord.y - boardRoot.y) / boardStep);

        return new Vector2Int(xCoord, yCoord);
    }

    public static Vector2 VectorIntToCoords(Vector2Int coord)
    {
        float xCoord = (coord.x * boardStep) + boardRoot.x;
        float yCoord = (coord.y * boardStep) + boardRoot.x;

        return new Vector2(xCoord, yCoord);
    }

    public void PopulateBoard()
    {
        Vector2Int tileCoord = new Vector2Int(0, 0);

        for (int x = 0; x < Board.Tiles.GetLength(0); x++)
        {
            for (int y = 0; y < Board.Tiles.GetLength(1); y++)
            {
                tileCoord.Set(x, y);
                Vector2 coords = VectorIntToCoords(tileCoord);

                Board.Tiles[x, y] = new Tile(coords);
                Board.Tiles[x, y].Monster = Object.Instantiate(MonsterPrefab, BoardParent);
                Board.Tiles[x, y].Monster.SetPosition(coords);
                MonsterType type = MonsterTypes[Random.Range(0, MonsterTypes.Length)];
                Board.Tiles[x, y].Monster.SetMonsterType(type);
            }
        }
    }

    public IEnumerator RepopulateBoard()
    {
        float ySpawnPos = boardStep * ((boardHeight / 2) + 1);
        Vector2Int tileCoord = new Vector2Int(0, 0);

        for (int x = 0; x < Board.Tiles.GetLength(0); x++)
        {

            for (int y = 0; y < Board.Tiles.GetLength(1); y++)
            {
                if (Board.Tiles[x, y].Monster == null)
                {
                    tileCoord.Set(x, y);
                    Vector2 coords = VectorIntToCoords(tileCoord);

                    Board.Tiles[x, y].Monster = Object.Instantiate(MonsterPrefab, BoardParent);
                    Board.Tiles[x, y].Monster.SetPosition(coords);
                    MonsterType type = MonsterTypes[Random.Range(0, MonsterTypes.Length)];
                    Board.Tiles[x, y].Monster.SetMonsterType(type);
                }
            }
        }

        yield return new WaitForEndOfFrame();
    }

    public Tile[] GetTilesInRowOrColumn(Vector2 origin, Vector2 direction)
    {
        direction = direction.normalized;
        List<Tile> tiles = new List<Tile>();
        Vector2Int blockTileLocation = CoordsToVectorInt(origin);

        for (int x = 0; x < (Board.Tiles.GetLength(0) * direction.x); x++)
        {
            tiles.Add(Board.Tiles[x, blockTileLocation.y]);
        }

        for (int y = 0; y < (Board.Tiles.GetLength(1) * direction.y); y++)
        {
            tiles.Add(Board.Tiles[blockTileLocation.x, y]);
        }

        return tiles.ToArray();
    }

    /*
     * Board set up breakdown:
     * 1. Fill every other tile with a random monster
     * 2. Check the other tiles that there isn't a matching monster horizontally or vertically
     * 
     */

    //private int CountAdjacentSimilarMonsters(int tileXCoord, int tileYCoord)
    //{
        
    //}
}
