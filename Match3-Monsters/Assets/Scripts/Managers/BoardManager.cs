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

    public BoardManager()
    {
        Board = new Board(new Tile[boardWidth, boardHeight]);
    }

    public void PopulateBoard(MonsterBlock prefab, Transform parent)
    {
        for (int x = 0; x < Board.Tiles.GetLength(0); x++)
        {
            float xPos = (x * boardStep) + boardRoot.x;

            for (int y = 0; y < Board.Tiles.GetLength(1); y++)
            {
                float yPos = (y * boardStep) + boardRoot.y;
                Vector2 coords = new Vector2(xPos, yPos);

                Board.Tiles[x, y] = new Tile(coords);
                Board.Tiles[x, y].Monster = Object.Instantiate(prefab, parent);
                Board.Tiles[x, y].Monster.SetPosition(coords);
            }
        }
    }

    public static Vector2Int CoordsToVectorInt(Vector2 coord)
    {
        int xCoord = Mathf.RoundToInt((coord.x - boardRoot.x) / boardStep);
        int yCoord = Mathf.RoundToInt((coord.y - boardRoot.y) / boardStep);

        return new Vector2Int(xCoord, yCoord);
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
}
