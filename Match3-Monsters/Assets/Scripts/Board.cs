using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// This class represents the game board
/// </summary>
public class Board
{
    public Tile[,] Tiles { get; set; }

    public Board(Tile[,] tiles)
    {
        Tiles = tiles;
    }
}

/// <summary>
/// This class represents a tile on the game board
/// </summary>
public class Tile
{
    public Vector2 Coordinates { get; private set; }
    public MonsterBlock Monster { get; set; }

    public Tile(Vector2 coords)
    {
        Coordinates = coords;
    }
}
